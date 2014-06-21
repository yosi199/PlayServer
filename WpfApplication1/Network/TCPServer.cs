
using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PlayServer.MessageTypes;
using ServiceStack;


namespace PlayServer.Network
{
    // State object for reading client data asynchronously
    public class StateObject
    {
        // Client  socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 1024;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }

    public class SynchronousSocketListener
    {
        // Incoming data from the client.
        public static string data = null;
        private static PlayerUI mainW = null;
        private static Socket handler;

        public SynchronousSocketListener() { StartListening(); }

        public static void StartListening()
        {
            // Data buffer for incoming data.
            byte[] bytes = new Byte[1024];

            MessageManager messageHandler = MessageManager.Instance;
            messageHandler.registerUI(mainW);

            string messageReturned = string.Empty;

            // Establish the local endpoint for the socket.
            // Dns.GetHostName returns the name of the 
            // host running the application.
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 5555);

            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and 
            // listen for incoming connections.
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                // Start listening for connections.
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    // Program is suspended while waiting for an incoming connection.
                    handler = listener.Accept();
                    data = null;

                    // An incoming connection needs to be processed.
                    while (true)
                    {
                        bytes = new byte[1024];
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (data.IndexOf("<EOF>") > -1)
                        {
                            data = data.Replace("<EOF>", "");
                            messageReturned = messageHandler.figureMessageType(data);
                            break;
                        }
                    }

                    // Show the data on the console.
                    Console.WriteLine("Text received : {0}", data);

                    // Echo the data back to the client.


                    byte[] msg = Encoding.ASCII.GetBytes(messageReturned);

                    handler.Send(msg);
                    Console.WriteLine("Message sent: " + Encoding.ASCII.GetString(msg));
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();


                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                // StartListening();
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();

        }

        public static void registerUI(PlayerUI mainUI)
        {
            mainW = mainUI;
        }


    }
}