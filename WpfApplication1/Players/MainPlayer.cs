using PlayServer.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayServer.Network;

namespace PlayServer.Players
{
    /// <summary>
    ///  This class serves as the main player that communicates and control the
    ///  choosen players. A part of the strategy pattern.
    /// </summary>
    class MainPlayer : IPlayCommands
    {

        private IPlayCommands playerChoosen;
        private static MainPlayer mainPlayerInstance;


        private MainPlayer() { }
        public static MainPlayer Instance
        {
            get
            {
                if (mainPlayerInstance == null)
                {
                    mainPlayerInstance = new MainPlayer();

                }
                return mainPlayerInstance;
            }
        }

        public void setPlayer(IPlayCommands player)
        {
            playerChoosen = player;
        }


        public string Play()
        {
            string returnValue = playerChoosen.Play();
            AsynchronousSocketListener.freeToSend.Signal();

            return returnValue;
        }

        public void Stop()
        {
            playerChoosen.Stop();
        }

        public void Rewind()
        {
            playerChoosen.Rewind();
        }

        public void Forward()
        {
            playerChoosen.Forward();
        }
    }
}
