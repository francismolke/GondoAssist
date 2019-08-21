
using GondoAssist;
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
        public MainWindow()
        {
            InitializeComponent();
            maincontent.Children.Clear();
            UCYTSearch ucyts = new UCYTSearch(this);
            maincontent.Children.Add(ucyts);
        }

        /// <summary>
        /// Button: Öffnet im Fenster das Youtube Such Interface auf
        /// </summary>
        private void onYoutubeSearchClicked(object sender, RoutedEventArgs e)
        {
            maincontent.Children.Clear();
            UCYTSearch ucyts = new UCYTSearch(this);
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
            UCDownloader ucd = new UCDownloader(link);

            maincontent.Children.Add(test);


        }

        private void onIGLinkGrabberClicked(object sender, RoutedEventArgs e)
        {
            maincontent.Children.Clear();
            
        }
    }
}
