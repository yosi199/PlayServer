using PlayServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WpfApplication1.Player
{
    /// <summary>
    ///  Controls media playback logic
    /// </summary>
    class Player : IPlayCommands
    {

        private static FileManger instance = FileManger.Instance;

        private int _currentPosition = 0;

        public void Play()
        {
            try
            {
                // TODO - MUST HAVE CONDITION OBJECT LOCKING TO MAKE SURE LIST ISN'T EMPTY
                String filePAth = instance.FilesInfoList[_currentPosition].FullName.ToString();
                Console.WriteLine(filePAth);
            }


            catch (Exception ex)
            {
                Console.WriteLine(ex.TargetSite.ToString());
            }
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
