using CoreAudioApi;
using PlayServer.MessageTypes;
using PlayServer.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayServer.Network;
using System.Threading;
using ServiceStack;

namespace PlayServer.Players
{
    /// <summary>
    ///  This class serves as the main player that communicates and control the
    ///  choosen players. A part of the strategy pattern.
    /// </summary>
    class MainPlayer : IPlayCommands
    {

        private IPlayCommands playerChoosen;
        private MMDevice device;

        private static MainPlayer mainPlayerInstance;

        public static CountdownEvent mWaitForParsing;

        private MainPlayer()
        {
            mWaitForParsing = new CountdownEvent(1);          
        }

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

        /// <summary>
        /// Increment/Decrement master volume
        /// </summary>
        /// <param name="whichWay">Whether it should increment or decrement</param>
        /// <returns>Current Volume</returns>
        public string Volume(int newVolume, string whichWay)
        {
            string volume = string.Empty;
            MMDeviceEnumerator devEnum = new MMDeviceEnumerator();
            MMDevice defaultDevice = devEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);


            switch (whichWay)
            {
                case "Up":
                    for (int i = 0; i < newVolume; i++)
                    {
                        defaultDevice.AudioEndpointVolume.VolumeStepUp();
                    }
                    break;

                case "Down":
                    for (int i = 0; i < newVolume; i++)
                    {
                        defaultDevice.AudioEndpointVolume.VolumeStepDown();
                    }
                    break;
            }




            volume = defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar.ToString();

            return volume;

        }


    }
}