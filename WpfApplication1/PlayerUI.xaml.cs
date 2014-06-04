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

namespace PlayServer
{
    /// <summary>
    /// Interaction logic for UI
    /// </summary>
    public partial class PlayerUI : Window
    {

        private FileManger fm;
        private AsyncSocketListener server;
        private PlayServer.Player.MediaPlayer player;

        public PlayerUI()
        {
            //get the instance of the file manager singelton and register the UI
            fm = FileManger.Instance;
            fm.registerUI(this);

            // Start the server and register the UI
            Task t = new Task(() => server = new AsyncSocketListener());
            AsyncSocketListener.registerUI(this);
            t.Start();

            // get the player instance
            player = PlayServer.Player.MediaPlayer.Instance;

        }

        /// <summary>
        /// opens load from directory dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadDirBtn(object sender, RoutedEventArgs e)
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

            }
        }

        /// <summary>
        /// A btn interface to start playing the track
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayBtn(object sender, RoutedEventArgs e)
        {
            player.Play();
        }

        public void UpdateUIFromNewThread(String message)
        {

            Dispatcher.BeginInvoke(
              System.Windows.Threading.DispatcherPriority.Normal,
             new Action(
        delegate()
        {
            loadBTN.IsEnabled = true;
            pb.IsIndeterminate = false;
            pb.IsEnabled = false;
            pbLabel.Content = string.Format(message, fm.filesCount, fm.foldersCount);
            Play.Visibility = Visibility.Visible;
            pbLabel.Focus();

        }
         ));

        }

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
