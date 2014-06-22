using System.Timers;
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
using Timer = System.Timers.Timer;

namespace PlayServer.Players
{
    /// <summary>
    ///  This class serves as the main player that communicates and control the
    ///  choosen players. A part of the strategy pattern.
    /// </summary>
    internal class MainPlayer : IPlayCommands
    {

        private IPlayCommands playerChoosen;
        private MMDevice device;
        private static Timer VolumeTimer;
        private int _volumeCalledCounter = 0;
        private Object _lock;

        private static MainPlayer mainPlayerInstance;

        public static CountdownEvent mWaitForParsing;



        private MainPlayer()
        {
            _lock = new object();
            mWaitForParsing = new CountdownEvent(1);
            MMDeviceEnumerator DevEnum = new MMDeviceEnumerator();
            device = DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            device.AudioEndpointVolume.OnVolumeNotification +=
                new AudioEndpointVolumeNotificationDelegate(AudioEndpointVolume_OnVolumeNotification);
            VolumeTimer = new Timer();
            VolumeTimer.Elapsed += new ElapsedEventHandler(VolumeTimer_Tick);
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

        public void SetPlayer(IPlayCommands player)
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

        private void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
        {
            // Play with this interval to change the rate of updates the server will send to the client on volume changes
            VolumeTimer.Interval = 10;
            VolumeTimer.Stop();
            VolumeTimer.Start();

        }

        private void VolumeTimer_Tick(object sender, EventArgs e)
        {
            VolumeTimer.Stop();
            string messageUpdate;

            lock (_lock)
            {
                ++_volumeCalledCounter;

                if (_volumeCalledCounter <= 1)
                {
                    _volumeCalledCounter = 0;

                    messageUpdate = new ServerStatusMessage().ToJson<ServerStatusMessage>();
                    SynchronousSocketListener.Send(messageUpdate);
                }
            }




        }
    }
}