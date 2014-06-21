using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreAudioApi;

namespace PlayServer.MessageTypes
{
    class ServerStatusMessage
    {

        private string _messageType = "Status";
        private float _currentVolume = 0;
        private float _maxVolume = 0;
        private float _minVolume = 0;

        public string messageType { get { return _messageType; } }
        public float CurrentVolume
        {
            get { return _currentVolume; }
            set { _currentVolume = value; }
        }


        public float MaxVolume
        {
            get { return _maxVolume; }
            set { _maxVolume = value; }
        }

        public float MinVolume
        {
            get { return _minVolume; }
            set { _minVolume = value; }
        }



        public ServerStatusMessage()
        {
            MMDeviceEnumerator devEnum = new MMDeviceEnumerator();
            MMDevice defaultDevice = devEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            CurrentVolume = defaultDevice.AudioEndpointVolume.MasterVolumeLevel;
            _maxVolume = defaultDevice.AudioEndpointVolume.VolumeRange.MaxdB;
            _minVolume = defaultDevice.AudioEndpointVolume.VolumeRange.MindB;

        }


    }
}
