using GondoAssist.Klassen;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using YoutubeSearch;


namespace GondoAssist
{
    /// <summary>
    /// Interaktionslogik für UCYTSearch.xaml
    /// </summary>public 
    public partial class UCYTSearch : UserControl
    {

        string link;
        VideoSearch items = new VideoSearch();
        List<Youtube> list = new List<Youtube>();
        MainWindow mw;

        public UCYTSearch(MainWindow mw)
        {
            this.mw = mw;
            InitializeComponent();
        }

        // Sucht nach dem eingegeben Suchwort und gibt diese im Datagrid aus
        private void OnSearchClicked(object sender, RoutedEventArgs e)
        {

            foreach (var item in items.SearchQuery(Searchbox.Text, 1))
            {
                Youtube video = new Youtube();
                video.Title = item.Title;
                video.Author = item.Author;
                video.Url = item.Url;
                link = item.Url;
                byte[] imageBytes = new WebClient().DownloadData(item.Thumbnail);
                BitmapImage bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = new MemoryStream(imageBytes);
                bmp.EndInit();
                video.Image = bmp;
                video.Thumbnail = bmp;
                list.Add(video);

            }

            dataGridView.ItemsSource = list;
        }
        // Der Prozess zum Öffnen des Browsers mit der URL
        static void browseSite(string url)
        {
            System.Diagnostics.Process.Start(url);
        }

        // Öffnet die Seite im Browser

        private void openInBrowser(object sender, RoutedEventArgs e)
        {

            browseSite(link);
        }
        // Event für das Selektieren eines Items im Datagrid
        private void OnSelect(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                link = (e.AddedItems[0] as Youtube).Url;
            }
        }


        // Download-Window wird aufgerufen und es wird ihm der link mitgegeben
        private void onDownloadClicked(object sender, RoutedEventArgs e)
        {
            mw.maincontent.Children.Clear();
            UCDownloader ucd = new UCDownloader(link);
            mw.maincontent.Children.Add(ucd);

        }
        // DÖffnet den DownloadDialog
        private void openDownloadDialog()
        {
            UCDownloader ucd = new UCDownloader(link);
            mw.maincontent.Children.Add(new UCDownloader(link));


        }
        // Tasten Event (Enter) für die Ausgabe der Videodetails im Datagrid
        string urlyt;
        private void onSearchEntered(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                foreach (var item in items.SearchQuery(Searchbox.Text, 1))
                {
                    Youtube video = new Youtube();
                    video.Title = item.Title;
                    video.Author = item.Author;
                    video.Url = item.Url;
                    link = item.Url;
                    byte[] imageBytes = new WebClient().DownloadData(item.Thumbnail);
                    BitmapImage bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.StreamSource = new MemoryStream(imageBytes);
                    bmp.EndInit();
                    video.Image = bmp;
                    video.Thumbnail = bmp;
                    list.Add(video);
                    urlyt = video.Url;
                }

                dataGridView.ItemsSource = list;
            }
        }

        // Öffnet im Browser das gewählte Item in Datagrid
        private void onYTVideoDoubClicked(object sender, MouseButtonEventArgs e)
        {
            browseSite(link);
        }

        private void onTimestampCreated(object sender, RoutedEventArgs e)
        {
            // nothing
        }

        // Kopiert den Link des gewähltem Item in Datagrid
        private void onYTVideoRightClicked(object sender, MouseButtonEventArgs e)
        {

            System.Windows.Forms.Clipboard.SetDataObject(link, true);
        }
    }

}

