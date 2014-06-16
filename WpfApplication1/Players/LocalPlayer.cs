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

        private static FileManger instance = FileManger.Instance;
        private static LocalMediaPlayerClass playerInstance;

        private object _locker = new object();

        private int _currentPosition = 0;
        private String jsonStringFile;

        private static System.Windows.Media.MediaPlayer mp = new System.Windows.Media.MediaPlayer();

        private LocalMediaPlayerClass() { }

        public static LocalMediaPlayerClass Instance
        {
            get
            {
                if (playerInstance == null)
                {
                    playerInstance = new LocalMediaPlayerClass();
                }
                return playerInstance;


            }
        }

        public string Play()
        {
            string returnValue = string.Empty;

            try
            {

                lock (_locker)
                {
                    if (instance.FilesInfoList.Count > 0)
                    {
                        // Get file path from Json
                        jsonStringFile = instance.FilesInfoList[_currentPosition].ToString();
                        returnValue = jsonStringFile;
                        Song currentSong = ServiceStack.Text.JsonSerializer.DeserializeFromString<Song>(jsonStringFile);

                        // Start playing
                        Uri track = new Uri(currentSong.pathInfo);
                        mp.Open(track);
                        mp.Play();


                    }
                }

            }


            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

            finally
            {
                MainPlayer.mWaitForParsing.Signal();
                MainPlayer.mWaitForParsing = new System.Threading.CountdownEvent(1);
            }

            return returnValue;

        }

        public bool Stop()
        {
            bool succeed;

            try
            {
                mp.Stop();
                succeed = true;

            }

            catch (Exception e) { succeed = false; }

            return succeed;

        }

        public string Rewind()
        {
            string returnValue = string.Empty;

            try
            {
                lock (_locker)
                {
                    if (_currentPosition > 0)
                    {
                        // Get previous file path from Json
                        jsonStringFile = instance.FilesInfoList[--_currentPosition].ToString();
                        returnValue = jsonStringFile;
                        Song currentSong = ServiceStack.Text.JsonSerializer.DeserializeFromString<Song>(jsonStringFile);

                        // Start playing
                        Uri track = new Uri(currentSong.pathInfo);
                        mp.Open(track);
                        mp.Play();

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

            finally
            {
                MainPlayer.mWaitForParsing.Signal();
                MainPlayer.mWaitForParsing = new System.Threading.CountdownEvent(1);

            }

            return returnValue;

        }

        public string Forward()
        {
            string returnValue = string.Empty;

            try
            {
                lock (_locker)
                {

                    if (_currentPosition < instance.FilesInfoList.Count())
                    {
                        // Get next file path from Json
                        jsonStringFile = instance.FilesInfoList[++_currentPosition].ToString();
                        Song currentSong = ServiceStack.Text.JsonSerializer.DeserializeFromString<Song>(jsonStringFile);

                        // Start playing
                        Uri track = new Uri(currentSong.pathInfo);
                        mp.Open(track);
                        mp.Play();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

            finally
            {
                MainPlayer.mWaitForParsing.Signal();
                MainPlayer.mWaitForParsing = new System.Threading.CountdownEvent(1);

            }

            return returnValue;

        }
    }
}

