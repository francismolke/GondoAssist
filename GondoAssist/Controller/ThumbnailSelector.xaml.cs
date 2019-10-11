using GondoAssist.EditForms;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GondoAssist
{
    /// <summary>
    /// Interaktionslogik für ThumbnailSelector.xaml
    /// </summary>
    public partial class ThumbnailSelector : UserControl
    {
        MainWindow mw_parent;
        int selectedColumn, selectedRow;
        string tnTitle, tnUrl;
        ImageSource tnThumbnail;
        string imageElement = "Image";
        string titleElement = "Title";
        string UrlElement = "Url";
        int gridCol = 0;
        int gridRow = 0;
        int rowPos, columnPos, n = 0;

        private void SelectedThumbnail(object sender, RoutedEventArgs e)
        {

        }

        //    YouTubeService youtubeservice = new YouTubeService();
        public ThumbnailSelector(MainWindow mw)
        {
            InitializeComponent();
            mw_parent = mw;
        }



        // Buttonevent zum öffnen des Bearbeitungsfensters für ThumbnailSelector
        private void AddEditTN(object sender, RoutedEventArgs e)
        {
            ThumbnailSelectEditForm teF = new ThumbnailSelectEditForm(mw_parent);
            teF.Owner = mw_parent;


            teF.Closed += TSEFWindow_Closed;
            teF.Show();
        }

        private void MakeElementName()
        {

        }
        private void TSEFWindow_Closed(object sender, EventArgs e)
        {
            MessageBox.Show("passt" + mw_parent.selectedRow + "x" + mw_parent.selectedColumn);
            //  MessageBox.Show("Something has happened" + selectedRow + "" + selectedColumn, "Parent");
            this.selectedColumn = mw_parent.selectedColumn;
            this.selectedRow = mw_parent.selectedRow;
            this.tnTitle = mw_parent.tnTitle;
            this.tnUrl = mw_parent.tnUrl;
            this.tnThumbnail = mw_parent.tnThumbnail;
            //Image0x0.Source = mw_parent.tnThumbnail;
            //Title0x0.Content = mw_parent.tnTitle;
            //Url0x0.Content = mw_parent.tnUrl;
            imageElement = "Image";
            imageElement = imageElement + gridRow.ToString() + "x" + gridCol.ToString();

            // String Builder für Title
            titleElement = "Title";
            titleElement = titleElement + gridRow.ToString() + "x" + gridCol.ToString();
            // String Builder für Url
            UrlElement = "Url";
            UrlElement = UrlElement + gridRow.ToString() + "x" + gridCol.ToString();

            if (gridRow < 2)
            {
                if (columnPos <= 4)
                {
                    if (columnPos <= 3)
                    {
                        Image imgx = this.FindName(imageElement) as Image;
                        Label lbtitle = this.FindName(titleElement) as Label;
                        Label lburl = this.FindName(UrlElement) as Label;
                        imgx.Source = mw_parent.tnThumbnail;
                        lbtitle.Content = mw_parent.tnTitle;
                        lburl.Content = mw_parent.tnUrl;
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
            else
            {
                gridRow -= gridRow;

            }
           
        //    rowPos -= rowPos;

        }
    }
}
