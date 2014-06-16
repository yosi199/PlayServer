using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Text;
using PlayServer.Players;

namespace PlayServer.Network
{
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


        public string figureMessageType(string message)
        {
            string returnedValue = string.Empty;
            if (message.Length > 0)
            {
                var messageObj = JsonObject.Parse(message);
                string type = messageObj.Get("MessageType");

                switch (type)
                {
                    case "Play": mainW.Dispatcher.Invoke(new Action(() => returnedValue = player.Play())); break;
                    case "Stop": mainW.Dispatcher.Invoke(new Action(() => player.Stop())); break;
                    case "Rewind": mainW.Dispatcher.Invoke(new Action(() => returnedValue = player.Rewind())); break;
                    case "Forward": mainW.Dispatcher.Invoke(new Action(() => returnedValue = player.Forward())); break;

                }
            }


            return returnedValue;
        }

    }
}
