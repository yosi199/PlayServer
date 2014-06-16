using PlayServer.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayServer.Network;
using System.Threading;

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

        public static CountdownEvent mWaitForParsing;

        private MainPlayer() { mWaitForParsing = new CountdownEvent(1); }

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
            return returnValue;
        }

        public bool Stop()
        {
            bool success = playerChoosen.Stop();

            return success;
        }

        public string Rewind()
        {
            string returnValue = playerChoosen.Rewind();
            return returnValue;
        }

        public string Forward()
        {
            string returnValue = playerChoosen.Forward();
            return returnValue;
        }
    }
}
