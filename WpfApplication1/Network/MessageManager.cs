using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayServer.MessageTypes;
using PlayServer.Player;
using ServiceStack;
using ServiceStack.Text;
using PlayServer.Players;

namespace PlayServer.Network
{
    /// <summary>
    /// A Controller class that reads the message and perform 
    /// an action based on it's type
    /// </summary>
    class MessageManager
    {

        private static MessageManager instance = null;
        private MainPlayer player;
        private PlayerUI mainW;

        private MessageManager()
        {
            player = MainPlayer.Instance;

        }

        public static MessageManager Instance
        {

            get
            {
                if (instance == null)
                {
                    return instance = new MessageManager();
                }

                return instance;
            }

        }

        public void registerUI(PlayerUI main)
        {
            mainW = main;
        }

        /// <summary>
        /// Performs actions based on message type
        /// </summary>
        /// <param name="message">A JSON string that encapsulate the command</param>
        /// <returns>passes a string back to the server</returns>
        public string FigureMessageType(string message)
        {
            string returnedValue = string.Empty;
            if (message.Length > 0)
            {
                var messageObj = JsonObject.Parse(message);
                string type = messageObj.Get("MessageType");

                switch (type)
                {

                    // Choose player
                    case "SetPlayer":
                        break;

                    // Player Control related messages
                    case "Play": mainW.Dispatcher.Invoke(() => returnedValue = player.Play()); break;
                    case "Stop": mainW.Dispatcher.Invoke(() => player.Stop()); break;
                    case "Backward": mainW.Dispatcher.Invoke(() => returnedValue = player.Rewind()); break;
                    case "Forward": mainW.Dispatcher.Invoke(() => returnedValue = player.Forward()); break;
                    case "Shuffle":
                        bool shuffle = Boolean.Parse(messageObj.Get("IsShuffleOn"));
                        mainW.Dispatcher.Invoke(() => player.SetShuffle(shuffle));
                        returnedValue = new ServerStatusMessage().ToJson<ServerStatusMessage>();
                        break;

                    case "CurrentlyPlaying":
                        mainW.Dispatcher.Invoke(() => returnedValue = player.GetCurrentSongJSON());
                        break;

                    // Computer Controls related messages
                    case "DeviceInfo": mainW.Dispatcher.Invoke(() => mainW.SocketInfo.Content = messageObj.Get("deviceName"));
                        returnedValue = new ServerStatusMessage().ToJson<ServerStatusMessage>();
                        break;
                    case "Volume":
                        mainW.Dispatcher.Invoke(() => returnedValue = player.Volume(messageObj.Get("progress").ToInt(), messageObj.Get("WhichWay")));
                        break;

                    case "KillAndRestart":
                       // SynchronousSocketListener.ResetServer();
                        new SynchronousSocketListener();
                        break;

                }
            }

            return returnedValue;
        }

    }
}