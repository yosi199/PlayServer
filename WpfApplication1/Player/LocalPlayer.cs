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

namespace PlayServer.Player
{
    /// <summary>
    ///  Controls media playback logic
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

        private void initPlayer()
        {
            try
            {
                // TODO - MUST HAVE CONDITION OBJECT LOCKING TO MAKE SURE LIST ISN'T EMPTY

                lock (_locker)
                {
                    if (instance.FilesInfoList.Count > 0)
                    {
                        // Get file path from Json
                        jsonStringFile = instance.FilesInfoList[_currentPosition].ToString();
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
                Console.WriteLine(ex.TargetSite.ToString());
            }

            finally { }
        }



        public void Play()
        {
            initPlayer();

        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Rewind()
        {
            throw new NotImplementedException();
        }

        public void Forward()
        {
            throw new NotImplementedException();
        }
    }
}

