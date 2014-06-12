using PlayServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Media;
using PlayServer.Files;

namespace PlayServer.Player
{
    /// <summary>
    ///  Controls media playback logic
    /// </summary>
    class MediaPlayerController : IPlayCommands
    {

        private static FileManger instance = FileManger.Instance;
        private static MediaPlayerController playerInstance;

        private object _locker = new object();

        private int _currentPosition = 0;
        private String filePAth;

        private static System.Windows.Media.MediaPlayer mp = new System.Windows.Media.MediaPlayer();


        private MediaPlayerController() { }

        public static MediaPlayerController Instance
        {
            get
            {
                if (playerInstance == null)
                {
                    playerInstance = new MediaPlayerController();
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
                        filePAth = instance.FilesInfoList[_currentPosition].FullName.ToString();
                        Uri track = new Uri(filePAth);
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
            try
            {
                mp.Stop();
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message.ToString());
            }
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
