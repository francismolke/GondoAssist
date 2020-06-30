using GondoAssist.Controller;
using GondoAssist.EditForms;
using Squirrel;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GondoAssist
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Window initialisierung

        public int selectedColumn { get; set; }
        public int selectedRow { get; set; }
        public string tnTitle { get; set; }
        public ImageSource tnThumbnail { get; set; }
        public string tnUrl { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            maincontent.Children.Clear();
            //   UCYTSearch ucyts = new UCYTSearch(this);
            //  maincontent.Children.Add(ucyts);
            AddVersionNumber();
            // CheckForUpdates();

        }

        private async void CheckIfUpdate(object sender, RoutedEventArgs e)
        {
            await UpdateIfAvailable();
        }

        private void AddVersionNumber()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            this.Title += $" v.{ versionInfo.FileVersion}";


        }

        //private async Task CheckForUpdates()
        //{

        //    using (var manager = new UpdateManager(@"C:\Users\Agrre\Desktop\Releases"))
        //    {
        //        await manager.UpdateApp();
        //    }
        //    //MessageBox.Show("Updated.");
        //}

        public async Task UpdateIfAvailable()
        {
            updateInProgress = RealUpdateIfAvailable();
            await updateInProgress;
        }

        public async Task WaitForUpdatesOnShutdown()
        {
            // We don't actually care about errors here, only completion
            await updateInProgress.ContinueWith(ex => { });
        }

        Task updateInProgress = Task.FromResult(true);
        private DateTime lastUpdateCheck;

        private async Task RealUpdateIfAvailable()
        {
            
            //using (var mgr = new UpdateManager("https://github.com/francismolke/GondoAssist"))
            using (var mgr = await UpdateManager.GitHubUpdateManager("https://github.com/francismolke/GondoAssist"))  
            {
                // this.logger.Info("Checking for updates");
                try
                {
                    var updateInfo = await mgr.CheckForUpdate();

                    if (updateInfo.ReleasesToApply.Any())
                    {
                        var versionCount = updateInfo.ReleasesToApply.Count;

                        var versionWord = versionCount > 1 ? "versions" : "version";
                        var message = new StringBuilder().AppendLine($"Die App ist {versionCount} {versionWord} zurück.").
                                                          AppendLine("Wenn Sie sich für eine Aktualisierung entscheiden, werden Änderungen erst nach einem Neustart der App wirksam.").
                                                          AppendLine("Möchten Sie sie herunterladen und installieren?").
                                                          ToString();

                        var result = MessageBox.Show(message, "App Update", MessageBoxButton.YesNo);
                        if (result != MessageBoxResult.Yes)
                        {
                            return;
                        }

                        var updateResult = await mgr.UpdateApp();
                    }
                    else
                    {
                        MessageBox.Show("Die App ist auf den neuesten Stand.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    //       this.logger.Warn($"There was an issue during the update process! {ex.Message}");
                }
            }
            //lastUpdateCheck = DateTime.Now;
            //try
            //{
            //    using (var mgr = UpdateManager.GitHubUpdateManager("https://github.com/francismolke/GondoAssist"))
            //    {
            //        await mgr.Result.UpdateApp();
            //        MessageBox.Show("Updated.");
            //    }
            //    //using (var mgr = new UpdateManager(@"C:\Users\Agrre\Desktop\Releases"))
            //    //{
            //    //    await mgr.UpdateApp();
            //    //}

            //}

            //catch (Exception ex)
            //{
            //    using (StreamWriter sw = new StreamWriter("Error.txt", true, Encoding.UTF8))
            //    {
            //        sw.WriteLine(ex.Message);
            //    }
            //}
        }
        /// <summary>
        /// Button: Öffnet im Fenster das Youtube Such Interface auf
        /// </summary>
        private void onYoutubeSearchClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                maincontent.Children.Clear();
                UCYTSearch ucyts = new UCYTSearch(this);
                //  Uploader_Youtube ucyts = new Uploader_Youtube(this);
                maincontent.Children.Add(ucyts);

            }
            catch (Exception ex)
            {
                using (StreamWriter sw = new StreamWriter("Error.txt", true, Encoding.UTF8))
                {
                    sw.WriteLine(ex.Message);
                }
            }
        }

        private void onYoutubeUploaderClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                maincontent.Children.Clear();
                Uploader_Youtube ucyts = new Uploader_Youtube(this);
                maincontent.Children.Add(ucyts);

            }
            catch (Exception ex)
            {
                using (StreamWriter sw = new StreamWriter("Error.txt", true, Encoding.UTF8))
                {
                    sw.WriteLine(ex.Message);
                }
            }
        }

        // Button-Click Event: Öffnet das Upload-Interface 
        private void onUploaderClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                maincontent.Children.Clear();
                UCUploader ucu = new UCUploader();
                maincontent.Children.Add(ucu);
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = new StreamWriter("Error.txt", true, Encoding.UTF8))
                {
                    sw.WriteLine(ex.Message);
                }
            }
        }

        // Button-Click Event: Öffnet das Download-Interface 
        private void onDownloaderClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                maincontent.Children.Clear();
                string link = "";
                UCDownloader ucd = new UCDownloader(link);
                maincontent.Children.Add(ucd);
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = new StreamWriter("Error.txt", true, Encoding.UTF8))
                {
                    sw.WriteLine(ex.Message);
                }
            }
        }

        private void onWordpressClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                maincontent.Children.Clear();
                Uploader_Wordpress uwp = new Uploader_Wordpress();
                maincontent.Children.Add(uwp);
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = new StreamWriter("Error.txt", true, Encoding.UTF8))
                {
                    sw.WriteLine(ex.Message);
                }
            }
        }

        private void onGoogleDriveClicked(object sender, RoutedEventArgs e)
        {
            try
            {

                maincontent.Children.Clear();
                Uploader_GoogleDrive ugd = new Uploader_GoogleDrive();
                maincontent.Children.Add(ugd);
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = new StreamWriter("Error.txt", true, Encoding.UTF8))
                {
                    sw.WriteLine(ex.Message);
                }
            }

        }

        // Button-Click Event: Öffnet das Download-Interface 
        //public void testxD()
        //{
        //    maincontent.Children.Clear();
        //    UCDownloader ucd = new UCDownloader("https://www.youtube.com/watch?v=s49G6ph4XXA");
        //    maincontent.Children.Add(ucd);

        //}

        // Button-Click Event: cleared das Interface 
        private void onTest1Clicked(object sender, RoutedEventArgs e)
        {
            try
            {

                maincontent.Children.Clear();
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = new StreamWriter("Error.txt", true, Encoding.UTF8))
                {
                    sw.WriteLine(ex.Message);
                }
            }

        }

        private void onNewDownloaderClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                maincontent.Children.Clear();
                TestDownloader test = new TestDownloader();
                // UCDownloader test = new UCDownloader(link);

                maincontent.Children.Add(test);
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = new StreamWriter("Error.txt", true, Encoding.UTF8))
                {
                    sw.WriteLine(ex.Message + ex.ToString());
                    // Get stack trace for the exception with source file information
                    var st = new StackTrace(ex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(0);
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();
                    sw.WriteLine(st + " " + frame + " " + line + " ");
                }
            }


        }

        //private void onIGLinkGrabberClicked(object sender, RoutedEventArgs e)
        //{
        //    maincontent.Children.Clear();

        //}

        private void onInstagramGrabberClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                maincontent.Children.Clear();
                InstagramGrabber igu = new InstagramGrabber();
                maincontent.Children.Add(igu);
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = new StreamWriter("Error.txt", true, Encoding.UTF8))
                {
                    sw.WriteLine(ex.Message + ex.ToString());
                    // Get stack trace for the exception with source file information
                    var st = new StackTrace(ex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(0);
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();
                    sw.WriteLine(st + " " + frame + " " + line + " ");
                }
            }
        }

        // private void XonYoutubeSearchClicked(object sender, RoutedEventArgs e)
        private void onUploaderIconClicked(object sender, RoutedEventArgs e)
        {

            var element = (UIElement)e.Source;
            int c = Grid.GetColumn(element);
            int r = Grid.GetRow(element);
            //     if (e.Source.)
            foreach (UIElement control in MenuGrid.Children)
            {
                if (Grid.GetRow(control) == r && Grid.GetColumn(control) == c)
                {
                    control.Visibility = Visibility.Hidden;
                    //Remove(control);
                    break;
                }
            }
            AddSecondaryMenuGrid(c, r);

        }

        private void AddSecondaryMenuGrid(int c, int r)
        {
            SPUploader.SetValue(Grid.ColumnProperty, c);
            SPUploader.SetValue(Grid.RowProperty, r);
            SPUploader.Visibility = Visibility.Visible;
        }

        private void HideSecondaryMenuGrid(int c, int r)
        {
            SPUploader.SetValue(Grid.ColumnProperty, c);
            SPUploader.SetValue(Grid.RowProperty, r);
            SPUploader.Visibility = Visibility.Hidden;
        }

        private void UploaderMouseLeave(object sender, RoutedEventArgs e)
        {
            var element = (UIElement)e.Source;
            int c = Grid.GetColumn(element);
            int r = Grid.GetRow(element);
            foreach (UIElement control in MenuGrid.Children)
            {
                if (Grid.GetRow(control) == r && Grid.GetColumn(control) == c)
                {
                    control.Visibility = Visibility.Visible;
                    //Remove(control);
                    break;
                }
            }
            HideSecondaryMenuGrid(c, r);

        }

        private void ThumbnailEditFormClicked(object sender, RoutedEventArgs e)
        {

            ThumbnailSelectEditForm teF = new ThumbnailSelectEditForm(this);
            teF.Owner = this;


            teF.Closed += TSEFWindow_Closed;
            teF.Show();
        }

        private void TSEFWindow_Closed(object sender, EventArgs e)
        {
            MessageBox.Show("Mainwindow");
            // MessageBox.Show("Something has happened" + selectedRow + "" + selectedColumn, "Parent");
        }

        private void ThumbNailSelectedFormClicked(object sender, RoutedEventArgs e)
        {

            maincontent.Children.Clear();
            ThumbnailSelector teF = new ThumbnailSelector(this);
            maincontent.Children.Add(teF);


        }

        private void onAutoVideoClicked(object sender, RoutedEventArgs e)
        {

            //maincontent.Children.Clear();
            //InstagramGrabber igu = new InstagramGrabber();
            //maincontent.Children.Add(igu);
            maincontent.Children.Clear();
            AutoModeVideo amv = new AutoModeVideo();
            maincontent.Children.Add(amv);

        }


    }
}
