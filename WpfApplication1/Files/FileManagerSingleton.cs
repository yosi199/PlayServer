using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PlayServer.Utilities;

namespace PlayServer
{

    public class FileManger
    {
        private static FileManger instance;
        private PlayerUI mainW = null;
        private int fileCount;
        private int folderCount;

        // Lists for collecting files and folders data

        // TODO - MUST SYNCHRONIZE ACCESS TO THESE LISTS. PROBABLY USING "ReaderWriterLockSlim" CLASS AND CONDITION OBJECTS
        private List<FileInfo> files = new List<FileInfo>();
        private List<DirectoryInfo> folders = new List<DirectoryInfo>();

        private Object _locker = new Object();


        // public properties
        public int filesCount { get { return fileCount; } }
        public int foldersCount { get { return folderCount; } }
        public List<FileInfo> FilesInfoList { get { return files; } }

        private FileManger() { }

        public static FileManger Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FileManger();
                }
                return instance;
            }
        }

        // Pass Ui instance to use the dispatcher
        public void registerUI(PlayerUI main)
        {
            this.mainW = main;
        }

        /// <summary>
        /// starts a new Task to load files asyncrounsly 
        /// </summary>
        /// <param name="di">DirectoryInfo to look for files in</param>
        public void loadFromDirAsync(DirectoryInfo di)
        {

            lock (_locker)
            {
                fileCount = 0;
                folderCount = 0;
                Task loadDirAsync = new Task(() => FullDirList(di, "*.mp3"));
                loadDirAsync.Start();
            }
        }



        private void FullDirList(DirectoryInfo dir, string searchPattern)
        {

            lock (_locker)
            {
                try
                {
                    foreach (FileInfo f in dir.GetFiles(searchPattern))
                    {
                        files.Add(f);
                        fileCount++;

                    }
                }


                catch
                {
                    //Console.WriteLine("Directory {0}  \n could not be accessed!!!!", dir.FullName);
                    return;
                }

                finally
                {
                    mainW.UpdateUIFromNewThread(Constants.Indexing_ProgressMSG);
                }


                // process each directory
                foreach (DirectoryInfo d in dir.GetDirectories())
                {
                    folders.Add(d);
                    folderCount++;
                    FullDirList(d, searchPattern);

                }


            }
        }

    }
}
