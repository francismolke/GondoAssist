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
using System.Windows.Shapes;
using YoutubeSearch;

namespace GondoAssist.EditForms
{
    /// <summary>
    /// Interaktionslogik für ThumbnailSelectEditForm.xaml
    /// </summary>
    public partial class ThumbnailSelectEditForm : Window
    {
        public ThumbnailSelectEditForm()
        {
            InitializeComponent();
        }

        string link;
        string urlyt;
        VideoSearch items = new VideoSearch();
        List<Youtube> list = new List<Youtube>();
        int rowPos, columnPos, n = 0;

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

        int gridCol = 0;
        int gridRow = 0;
        string imageElement = "Image";

        private void OnSearchClicked(object sender, RoutedEventArgs e)
        {
            list.Clear();
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
                //         SPxZero.DataContext = list;
                //     TNGrid.DataContext = list;
                // ExtentElement.Skip(n).FirstOrDefault();
                imageElement = "Image";
                imageElement = imageElement + gridRow.ToString() + "x" + gridCol.ToString();
                // Mache so lange, bis Position des Columns 5 ist(5 ist nicht drin)
                if (gridRow < 2)
                { 
                
                if (columnPos <= 4)
                {
                    // 
                    if (columnPos <= 3)
                    {
                        // TNGrid.DataContext = list.Skip(n).FirstOrDefault();
                        
                        Image imgx = this.FindName(imageElement) as Image;

                        imgx.Source = bmp;
                        columnPos++;
                        gridCol++;
                    }
                    else
                    {
                        if (rowPos <= 2)
                        {
                            rowPos++;
                            gridRow++;
                            columnPos -= columnPos;
                            gridCol -= gridCol;
                        }
                        else
                        {
                            columnPos = 5;
                            // Beenden
                        }
                    }
                    }
                }

            }
            gridRow -= gridRow;
            rowPos -= rowPos;


        }

    }
}
