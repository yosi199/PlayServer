using PlayServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PlayServer.Player;

namespace PlayServer.Network
{
    public class StateObject
    {

        //Client Socket
        public Socket workSocket = null;
        // Size of  recieved buffer
        public const int BufferSize = 1024;
        // Recieve buffer
        public byte[] buffer = new byte[BufferSize];
        // Recieved data string 
        public StringBuilder sb = new StringBuilder();

    }


    public class AsyncSocketListener
    {



        private static Socket listener;

        // Thread signal.
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        private static PlayerUI mainW;
        private static Player.MediaPlayerController player = Player.MediaPlayerController.Instance;



        public AsyncSocketListener()
        {
            StartListening();

        }

        public static void registerUI(PlayerUI main)
        {
            mainW = main;
        }

        public static void StartListening()
        {
            // Data buffer for incoming data
            byte[] bytes = new Byte[1024];

            // Establish the local endpoint for the socket
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 5555);

            // Create a TCP socket
            if (listener == null)
            {
                listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            else Console.WriteLine("Only 1 client is allowed at once");

            // Bind the socket to the local endpoint and listen for incoming connections

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    // set the event to nonsignaled state
                    allDone.Reset();

                    // Start an async socket to listen for connections
                    mainW.UpdateSocketLblInfo("LIstening");
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);



                    allDone.WaitOne();
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                StartListening();
            }


        }

        // TODO - I NEED TO LOCK IT FOR 1 CLIENT ONLY
        public static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            allDone.Set();

            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = null;



            try
            {
                handler = listener.EndAccept(ar);
                if (handler.Connected)
                {
                    mainW.UpdateSocketLblInfo("connected");

                }
            }

            catch (Exception e) { Console.WriteLine(e.ToString()); }

            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            int bytesRead = 0;



            // Read data from the client socket. 
            try
            {
                bytesRead = handler.EndReceive(ar);
                string getStringMessage = Encoding.ASCII.GetString(
              state.buffer, 0, bytesRead).ToString();


                switch (getStringMessage)
                {
                    case "play\n": mainW.Dispatcher.BeginInvoke(new Action(() => MediaPlayerController.Instance.Play()));
                        Send(handler, "Playing!");
                        break;
                    case "stop": mainW.Dispatcher.BeginInvoke(new Action(() => MediaPlayerController.Instance.Stop()));
                        Send(handler, "Stopped!"); break;
                    case "back": Send(handler, "back!"); break;
                    case "forward": Send(handler, "forward!"); break;
                    case "disconnect": handler.Close(); break;
                    case "connect": handler.Close(); break;



                }

                mainW.UpdateSocketLblInfo(Encoding.ASCII.GetString(
                        state.buffer, 0, bytesRead) + "Connected");
            }

            catch (Exception e) { Console.WriteLine(e.ToString()); }



            try
            {
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                   new AsyncCallback(ReadCallback), state);
            }

            catch (Exception ex) { Console.WriteLine(ex.ToString()); }


            if (bytesRead > 0)
            {
                //    There  might be more data, so store the data received so far.
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                //Check for end-of-file tag. If it is not there, read 
                //more data.
                content = state.sb.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    // All the data has been read from the 
                    // client. Display it on the console.
                    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
                        content.Length, content);
                    // Echo the data back to the client.
                    Send(handler, content);
                }
                else
                {

                    // Not all data received. Get more.
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }

            }
        }

        private static void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);      

        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

    }

}
