﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreAudioApi;
using PlayServer.Player;
using PlayServer.Players;

namespace PlayServer.MessageTypes
{
    class ServerStatusMessage
    {

        private string _messageType = "Status";
        private float _currentVolume = 0;
        private float _maxVolume = 0;
        private float _minVolume = 0;
        private bool _isShuffleOn = false;
        private string _playerTypeSet = string.Empty;
        private string _jsonSong = "Not playing";


        // Song info
        public string SongJson {
            get { return _jsonSong; }
            set { _jsonSong = value; }
        }

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

        public bool IsShuffleOn { get { return _isShuffleOn; } set { _isShuffleOn = value; } }

        public string PlayerTypeSet { get { return _playerTypeSet; } }



        public ServerStatusMessage()
        {
            MMDeviceEnumerator devEnum = new MMDeviceEnumerator();
            MMDevice defaultDevice = devEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);

            _currentVolume = defaultDevice.AudioEndpointVolume.StepInformation.Step;
            _maxVolume = defaultDevice.AudioEndpointVolume.StepInformation.StepCount;
            _minVolume = 0;
            _isShuffleOn = LocalMediaPlayerClass._isShuffle;
            _playerTypeSet = MainPlayer.CurrentPlayerSet;
            SongJson = LocalMediaPlayerClass.GetCurrentSongPlaying;
        
        }


    }
}
