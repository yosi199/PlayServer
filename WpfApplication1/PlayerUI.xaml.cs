using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using PlayServer.Network;
using PlayServer.Player;
using PlayServer.Files;
using PlayServer.Utilities;
using PlayServer.Players;

namespace PlayServer
{
    /// <summary>
    /// Interaction logic for UI
    /// </summary>
    public partial class PlayerUI : Window
    {

        private FileManger fm;
        private SynchronousSocketListener server;
        private MainPlayer player;

        private Boolean _readyToPlay = true;

        public PlayerUI()
        {

            InitializeComponent();

            //get the instance of the file manager singelton and register the UI
            fm = FileManger.Instance;
            fm.registerUI(this);

            // Start the server and register the UI
            Task t = new Task(() => server = new SynchronousSocketListener());
            t.Start();

            SynchronousSocketListener.registerUI(this);

            // get the player instance
            player = MainPlayer.Instance;
            player.setPlayer(LocalMediaPlayerClass.Instance);
            LocalMediaPlayerClass.RegisterUi(this);

            // Set app version in title
            SetVersion();

        }

        /// <summary>
        /// opens load from directory dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadDirBtn(object sender, RoutedEventArgs e)
        {


            // Create a dialog to choose dir and load songs in a new thread
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();


            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // Call File Manager Async file loader
                fm.loadFromDirAsync(new DirectoryInfo(dialog.SelectedPath));
                // Update UI
                pb.IsIndeterminate = true;
                pb.IsEnabled = true;
                loadBTN.IsEnabled = false;
                pbLabel.Content = "Loading...";

            }
        }

        /// <summary>
        /// A btn interface to start playing the track
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayBtn(object sender, RoutedEventArgs e)
        {
            // If nothing is being played, clicking the button will start to play
            if (_readyToPlay)
            {
                player.Play();
                Play.Content = "Stop";
                _readyToPlay = false;
            }

                // If already playing, stop it and change btn text
            else if (!_readyToPlay)
            {
                player.Stop();
                Play.Content = "Play";
                _readyToPlay = true;
            }
        }

        private void SetVersion()
        {
            // Get auto incremented version number and display at title
            string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Title = String.Format(Constants.title, version);
        }

        public void UpdateUiFromNewThread(String message)
        {

            Dispatcher.BeginInvoke(
              System.Windows.Threading.DispatcherPriority.Normal,
             new Action(
        delegate()
        {
            loadBTN.IsEnabled = true;
            pb.IsIndeterminate = false;
            pb.IsEnabled = false;
            Play.Visibility = Visibility.Visible;
            pbLabel.Focus();

            try
            {
                pbLabel.Content = string.Format(message, fm.filesCount, fm.foldersCount);
            }

            catch (Exception e)
            {
                pbLabel.Content = "";
                Console.WriteLine(e.Message.ToString());
            }

        }
         ));

        }

        /// <summary>
        /// Updates communication label 
        /// </summary>
        /// <param name="message"></param>
        public void UpdateSocketLblInfo(string message)
        {
            Dispatcher.Invoke(
           System.Windows.Threading.DispatcherPriority.Normal,
          new Action(
              delegate()
              {
                  SocketInfo.Content = message;

              }
      ));

        }


    }
}