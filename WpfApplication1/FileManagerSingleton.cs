using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WpfApplication1
{

    public class FileManger
    {
        private static FileManger instance;
        private Thread loadFiles = null;
        private MainWindow mainW = null;
        private int fileCount = 0;
        private int folderCount = 0;


        public int filesCount { get { return fileCount; } }
        public int foldersCount { get { return folderCount; } }


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
        public void registerUI(MainWindow main)
        {
            this.mainW = main;
        }

        /// <summary>
        /// starts a new thread to load files asyncrounsly 
        /// </summary>
        /// <param name="di">DirectoryInfo to look for files in</param>
        public void loadFromDirAsync(DirectoryInfo di)
        {
            loadFiles = new Thread(() => FullDirList(di, "*.mp3"));
            loadFiles.Start();
            loadFiles.Name = "loadFromDirAsync";

        }

        // Lists for collecting files and folders data
        List<FileInfo> files = new List<FileInfo>();
        List<DirectoryInfo> folders = new List<DirectoryInfo>();

        private void FullDirList(DirectoryInfo dir, string searchPattern)
        {

            try
            {
                foreach (FileInfo f in dir.GetFiles(searchPattern))
                {
                    files.Add(f);
                    fileCount++;
                    //Console.WriteLine("added - " + f.ToString());

                    // Update progressBar Label
                    mainW.Dispatcher.Invoke(
                      System.Windows.Threading.DispatcherPriority.Normal,
                     new Action(
                delegate()
                {
                    mainW.pbLabel.Content = "Indexing - " + f.ToString();

                }
                 ));

                }
            }

            catch
            {
                Console.WriteLine("Directory {0}  \n could not be accessed!!!!", dir.FullName);
                return;
            }


            // process each directory
            foreach (DirectoryInfo d in dir.GetDirectories())
            {
                folders.Add(d);
                folderCount++;
                FullDirList(d, searchPattern);
            }

 
        }

        /// <summary>
        /// Checks if the async file loading thread still runs
        /// </summary>
        /// <returns>bool running or not</returns>
        public bool isThreadStillLoading()
        {
            return (loadFiles.IsAlive);
        }



    }
}
