using GondoAssist.Klassen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
using YoutubeSearch;

namespace GondoAssist.EditForms
{
    /// <summary>
    /// Interaktionslogik für ThumbnailEditForm.xaml
    /// </summary>
    public partial class ThumbnailEditForm : Page
    {
        string link;
        string urlyt;
        VideoSearch items = new VideoSearch();
        List<Youtube> list = new List<Youtube>();
        public ThumbnailEditForm()
        {
            InitializeComponent();
        }

        private void onSearchEntered(object sender, KeyEventArgs e)
        {
            Youtube video = new Youtube();
            if (e.Key == Key.Enter)
            {
                foreach (var item in items.SearchQuery(Searchbox.Text, 1))
                {
                    
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
             //   GetUserCred();
                //dataGridView.ItemsSource = list;
            }

        }

        private void OnSearchClicked(object sender, RoutedEventArgs e)
        {
            Youtube video = new Youtube();
            foreach (var item in items.SearchQuery(Searchbox.Text, 1))
            {
                
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
            string testest = video.Title;

        }
    }
}
