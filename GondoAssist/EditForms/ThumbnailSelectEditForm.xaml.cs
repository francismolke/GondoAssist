using GondoAssist.Klassen;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using YoutubeSearch;

namespace GondoAssist.EditForms
{
    /// <summary>
    /// Interaktionslogik für ThumbnailSelectEditForm.xaml
    /// </summary>
    public partial class ThumbnailSelectEditForm : Window
    {

        int gridCol = 0;
        int gridRow = 0;
        string imageElement = "Image";
        string titleElement = "Title";
        string UrlElement = "Url";
        string buttonname = "";
        public int c, r;
        MainWindow mw_parent;
        BitmapImage superbmp;

        public ThumbnailSelectEditForm(MainWindow mw)
        {
            InitializeComponent();
            mw_parent = mw;
        }



        string link;
        string urlyt;
        VideoSearch items = new VideoSearch();
        List<Youtube> list = new List<Youtube>();
        int rowPos, columnPos, n = 0;

        private void SelectedThumbnail(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            buttonname = button.Name;
            string ganzeposition = buttonname.Substring(6, 3);
            r = int.Parse(ganzeposition.Substring(0, 1));
            c = int.Parse(ganzeposition.Substring(2, 1));
            GetTNPos(c, r);
        }

        static (int, int) GetTNPos(int c, int r)
        {
            return (c, r);
        }


        private void AddToTNBoard(object sender, RoutedEventArgs e)
        {
            //  c = c + c;
            //    r = r + 1 + r;
            MessageBoxResult result = MessageBox.Show("Wollen Sie dieses Video zum Board hinzufügen?", "Thumbnail Selector", MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    // Übergib properties zum anderen fenster ins erste
                    mw_parent.selectedRow = r;
                    mw_parent.selectedColumn = c;

                    //
                    imageElement = "Image";
                    imageElement = imageElement + r.ToString() + "x" + c.ToString();

                    // String Builder für Title
                    titleElement = "Title";
                    titleElement = titleElement + r.ToString() + "x" + c.ToString();
                    // String Builder für Url
                    UrlElement = "Url";
                    UrlElement = UrlElement + r.ToString() + "x" + c.ToString();

                    Image imgx = this.FindName(imageElement) as Image;
                    Label lbtitle = this.FindName(titleElement) as Label;
                    Label lburl = this.FindName(UrlElement) as Label;

                    mw_parent.tnTitle = lbtitle.Content as string;
                    mw_parent.tnThumbnail = imgx.Source;
                    mw_parent.tnUrl = lburl.Content as string;
                    this.Close();
                    break;
                case MessageBoxResult.No:
                    // Abort Mission, will get them next itme...
                    break;
            }



        }

        private void onSearchEntered(object sender, KeyEventArgs e)
        {
            list.Clear();
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

                    // String Builder für Image
                    imageElement = "Image";
                    imageElement = imageElement + gridRow.ToString() + "x" + gridCol.ToString();

                    // String Builder für Title
                    titleElement = "Title";
                    titleElement = titleElement + gridRow.ToString() + "x" + gridCol.ToString();
                    // String Builder für Url
                    UrlElement = "Url";
                    UrlElement = UrlElement + gridRow.ToString() + "x" + gridCol.ToString();

                    // Mache so lange, bis Position des Columns 5 ist(5 ist nicht drin)
                    if (gridRow < 2)
                    {
                        if (columnPos <= 4)
                        {
                            if (columnPos <= 3)
                            {
                                Image imgx = this.FindName(imageElement) as Image;
                                Label lbtitle = this.FindName(titleElement) as Label;
                                Label lburl = this.FindName(UrlElement) as Label;
                                imgx.Source = bmp;
                                lbtitle.Content = item.Title;
                                lburl.Content = item.Url;
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
                                }
                            }
                        }
                    }
                }
                gridRow -= gridRow;
                rowPos -= rowPos;
            }

        }


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
                superbmp = bmp;
                list.Add(video);

                // String Builder für Image
                imageElement = "Image";
                imageElement = imageElement + gridRow.ToString() + "x" + gridCol.ToString();

                // String Builder für Title
                titleElement = "Title";
                titleElement = titleElement + gridRow.ToString() + "x" + gridCol.ToString();
                // String Builder für Url
                UrlElement = "Url";
                UrlElement = UrlElement + gridRow.ToString() + "x" + gridCol.ToString();

                // Mache so lange, bis Position des Columns 5 ist(5 ist nicht drin)
                if (gridRow < 2)
                {
                    if (columnPos <= 4)
                    {
                        if (columnPos <= 3)
                        {
                            Image imgx = this.FindName(imageElement) as Image;
                            Label lbtitle = this.FindName(titleElement) as Label;
                            Label lburl = this.FindName(UrlElement) as Label;
                            imgx.Source = bmp;
                            lbtitle.Content = item.Title;
                            lburl.Content = item.Url;
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
                            }
                        }
                    }
                }

            }
            gridRow -= gridRow;
            rowPos -= rowPos;

            //    mw_parent.selectedColumn = c;
            //    mw_parent.selectedRow = r;
        }

    }
}
