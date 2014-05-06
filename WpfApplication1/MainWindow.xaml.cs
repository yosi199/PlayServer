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

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        FileManger fm;
        bool runCheck = true;



        public MainWindow()
        {
            fm = FileManger.Instance;
            fm.registerUI(this);
        }

        private void loadDirBtn(object sender, RoutedEventArgs e)
        {

            // Create a dialog to choose dir and load songs in a new thread
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            // Call File Manager Async file loader
            fm.loadFromDirAsync(new DirectoryInfo(dialog.SelectedPath));
            // Update UI
            pb.IsIndeterminate = true;
            pb.IsEnabled = true;
            loadBTN.IsEnabled = false;

            // Check if finished loading
            Thread t = new Thread(() => load());
            t.Start();
            t.Name = "LoaderThread";
        }

        private void load()
        {
            while (runCheck)
            {

                // If Async file loading thread finished...
                if (!fm.isThreadStillLoading())
                {
                    Console.WriteLine("Done running");

                    Dispatcher.Invoke(
                      System.Windows.Threading.DispatcherPriority.Normal,
                     new Action(
                delegate()
                {
                    loadBTN.IsEnabled = true;
                    pb.IsIndeterminate = false;
                    pb.IsEnabled = false;
                    runCheck = false;
                    pbLabel.Content = string.Format("Finished Indexing {0} files in {1} folder - Ready to play.", fm.filesCount, fm.foldersCount);

                }
                 ));

                }
            }
        }


    }
}
