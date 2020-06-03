
using GondoAssist;
using GondoAssist;
using GondoAssist.Controller;
using GondoAssist.EditForms;
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
        }

        /// <summary>
        /// Button: Öffnet im Fenster das Youtube Such Interface auf
        /// </summary>
        private void onYoutubeSearchClicked(object sender, RoutedEventArgs e)
        {
            maincontent.Children.Clear();
            UCYTSearch ucyts = new UCYTSearch(this);
          //  Uploader_Youtube ucyts = new Uploader_Youtube(this);
            maincontent.Children.Add(ucyts);
        }

        private void onYoutubeUploaderClicked(object sender, RoutedEventArgs e)
        {
            maincontent.Children.Clear();
            Uploader_Youtube ucyts = new Uploader_Youtube(this);
            maincontent.Children.Add(ucyts);
        }

        // Button-Click Event: Öffnet das Upload-Interface 
        private void onUploaderClicked(object sender, RoutedEventArgs e)
        {
            
            maincontent.Children.Clear();
            UCUploader ucu = new UCUploader();
            maincontent.Children.Add(ucu);
        }

        // Button-Click Event: Öffnet das Download-Interface 
        private void onDownloaderClicked(object sender, RoutedEventArgs e)
        {
            maincontent.Children.Clear();
            string link = "";
            UCDownloader ucd = new UCDownloader(link);
            maincontent.Children.Add(ucd);
        }

        private void onWordpressClicked(object sender, RoutedEventArgs e)
        {
            maincontent.Children.Clear();
            Uploader_Wordpress uwp = new Uploader_Wordpress();
            maincontent.Children.Add(uwp);
        }

        private void onGoogleDriveClicked(object sender, RoutedEventArgs e)
        {
            maincontent.Children.Clear();
            Uploader_GoogleDrive ugd = new Uploader_GoogleDrive();
            maincontent.Children.Add(ugd);

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
            maincontent.Children.Clear();

        }

        private void onNewDownloaderClicked(object sender, RoutedEventArgs e)
        {
            maincontent.Children.Clear();
            string link = "";
            TestDownloader test = new TestDownloader();
           // UCDownloader test = new UCDownloader(link);

            maincontent.Children.Add(test);


        }

        //private void onIGLinkGrabberClicked(object sender, RoutedEventArgs e)
        //{
        //    maincontent.Children.Clear();

        //}

        private void onInstagramGrabberClicked(object sender, RoutedEventArgs e)
        {
            maincontent.Children.Clear();
            InstagramGrabber igu = new InstagramGrabber();
            maincontent.Children.Add(igu);
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
                if (Grid.GetRow(control)==r && Grid.GetColumn(control) ==c)
                {
                    control.Visibility = Visibility.Hidden;
                        //Remove(control);
                    break;
                }
            }
            AddSecondaryMenuGrid(c,r);

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
            HideSecondaryMenuGrid(c,r);

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
