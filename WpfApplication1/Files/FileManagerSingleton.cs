using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PlayServer.Files;
using PlayServer.Utilities;
using ServiceStack;

namespace PlayServer.Files
{

    public class FileManger
    {
        private static FileManger instance;
        private PlayerUI mainW = null;
        private int fileCount;
        private int folderCount;

        // Lists for collecting files and folders data
        private List<string> files = new List<string>();
        private List<DirectoryInfo> folders = new List<DirectoryInfo>();

        // Locking and scheduling 
        private Object _locker = new Object();
        private CountdownEvent mCountDown;

        // public properties
        public int filesCount
        {
            get { return fileCount; }
        }

        public int foldersCount
        {
            get { return folderCount; }
        }

        public List<string> FilesInfoList
        {
            get { return files; }
        }

        private FileManger()
        {
        }

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
            mCountDown = new CountdownEvent(1);

            lock (_locker)
            {
                // Index all files to JSON in a list Asynchronosly 
                fileCount = 0;
                folderCount = 1;
                Task loadDirAsync = new Task(() => getMusicFromPath(di));
                loadDirAsync.Start();

                // Once Done, update user
                new Task(() => UpdateUI(Constants.Indexing_ProgressMSG)).Start();

            }


        }

        /// <summary>
        /// Updates UI when finished indexing files
        /// </summary>
        private void UpdateUI(String message)
        {
            mCountDown.Wait();
            mainW.UpdateUiFromNewThread(message);
        }

        private void getMusicFromPath(DirectoryInfo di)
        {
           // FullDirList(di, "*.mp3");
            GetFilesFromDir(di, "*.mp3");


            // Once previous method return, signal that job done
            // and progress can continue
            mCountDown.Signal();


        }

        private void FullDirList(DirectoryInfo dir, string searchPattern)
        {

            lock (_locker)
            {
                try
                {


                    foreach (FileInfo f in dir.GetFiles(searchPattern))
                    {
                        string artist = string.Empty;
                        string album = string.Empty;
                        string title = string.Empty;
                        string path = string.Empty;
                        int index;

                        string songJSON = string.Empty;

                        // Get song information and create a new Json object
                        try
                        {
                            TagLib.File tagFile = TagLib.File.Create(f.FullName.ToString());

                            if (tagFile.Tag.Performers.Count() > 0)
                            {
                                artist = tagFile.Tag.Performers[0];
                            }

                            album = tagFile.Tag.Album;
                            title = tagFile.Tag.Title;
                            path = f.FullName.ToString();
                            index = fileCount;


                            Song song = new Song(artist, album, title, path, index);
                            songJSON = song.ToJson<Song>();

                        }

                        catch (Exception e)
                        {
                            e.Message.ToString();
                        }


                        files.Add(songJSON);
                        fileCount++;

                    }
                }

                catch
                {
                    //Console.WriteLine("Directory {0}  \n could not be accessed!!!!", dir.FullName);
                    return;
                }

            }


            // process each directory
            foreach (DirectoryInfo d in dir.GetDirectories())
            {
                folders.Add(d);
                folderCount++;

                FullDirList(d, searchPattern);

            }


        }


        private void GetFilesFromDir(DirectoryInfo dir, string searchPattern)
        {

            foreach (FileInfo f in dir.GetFiles(searchPattern, searchOption: SearchOption.AllDirectories))
            {


                // Get song information and create a new Json object
                try
                {
                    int index = fileCount;
                    string artist = string.Empty;
                    string album;
                    string title;
                    string path;
                    string songJSON;


                    TagLib.File tagFile = TagLib.File.Create(f.FullName.ToString());

                    if (tagFile.Tag.Performers.Count() > 0)
                    {
                        artist = tagFile.Tag.Performers[0];
                    }

                    album = tagFile.Tag.Album;
                    title = tagFile.Tag.Title;
                    path = f.FullName.ToString();
            


                    Song song = new Song(artist, album, title, path, index);
                    songJSON = song.ToJson<Song>();

                    files.Add(songJSON);
                    fileCount++;

                    mainW.UpdateUiFromNewThread(artist + "- " + title);
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.Message.ToString());
                }
            }



        }
    }
}
