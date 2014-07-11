using PlayServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Media;
using ServiceStack;
using PlayServer.Files;
using PlayServer.Network;
using PlayServer.Players;

namespace PlayServer.Player
{
    /// <summary>
    ///  Controls local files, media playback logic
    /// </summary>
    class LocalMediaPlayerClass : IPlayCommands
    {

        // Singeltons instances;
        private static FileManger instance = FileManger.Instance;
        private PlayerUI mainW;

        // Lock Object
        private object _locker = new object();

        private int _currentPosition = 0;
        public static bool _isShuffle = false;
        private static String jsonStringFile;

        private static System.Windows.Media.MediaPlayer mp = new System.Windows.Media.MediaPlayer();

        public static String GetCurrentSongPlaying { get { return jsonStringFile; } }

        public LocalMediaPlayerClass(PlayerUI main)
        {
            mainW = main;
        }

        public string Play()
        {

            try
            {

                lock (_locker)
                {


                    if (instance.FilesInfoList.Count > 0)
                    {
                        // Get file path from Json
                        jsonStringFile = instance.FilesInfoList[_currentPosition];
                        Song currentSong = ServiceStack.Text.JsonSerializer.DeserializeFromString<Song>(jsonStringFile);

                        // Start playing
                        Uri track = new Uri(currentSong.pathInfo);
                        mp.Open(track);
                        mp.Play();

                        // Update Server UI
                        string currentlyPlayedSong = "Currently Playing: \n" + currentSong.artistName + " - " + currentSong.titleName;
                        mainW.Dispatcher.Invoke(
                            () => mainW.CurrPlayingLbl.Content = currentlyPlayedSong);

                        // Once the song finised playing, forward to the next one
                        mp.MediaEnded += (sender, e) => Forward();
                        // SynchronousSocketListener.Send(jsonStringFile);
                    }
                }

            }


            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            finally
            {
                MainPlayer.mWaitForParsing.Signal();
                MainPlayer.mWaitForParsing = new System.Threading.CountdownEvent(1);
            }

            return jsonStringFile;

        }

        public bool Stop()
        {
            bool succeed;

            try
            {
                mp.Stop();
                succeed = true;

            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                succeed = false;
            }

            return succeed;

        }

        public string Rewind()
        {

            try
            {
                lock (_locker)
                {
                    if (_currentPosition > 0)
                    {
                        // Get previous file path from Json
                        _currentPosition = --_currentPosition;
                        jsonStringFile = Play();

                    }

                    else if (_currentPosition == 0)
                    {
                        // Replay first song
                        jsonStringFile = Play();

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            finally
            {
                MainPlayer.mWaitForParsing.Signal();
                MainPlayer.mWaitForParsing = new System.Threading.CountdownEvent(1);

            }

            return jsonStringFile;

        }

        public string Forward()
        {

            lock (_locker)
            {

                if (_currentPosition < (instance.FilesInfoList.Count() - 1))
                {
                    if (_isShuffle)
                    {
                        Console.WriteLine(_currentPosition);
                        Random r = new Random();
                        _currentPosition = r.Next(0, instance.FilesInfoList.Count);
                    }

                    else
                    {
                        ++_currentPosition;
                    }
                    jsonStringFile = Play();

                }

                return jsonStringFile;
            }
        }

        public bool SetShuffle(bool isShuffleOn)
        {
            _isShuffle = isShuffleOn;
            return _isShuffle;
        }

        public string GetCurrentSongJSON()
        {
            return jsonStringFile;
        }
    }
}

