using GondoAssist.EditForms;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
        string imageElement;
        string titleElement;
        string UrlElement;
        int gridCol = 0;
        int gridRow = 0;
        int rowPos, columnPos, n = 0;
        private Brush _previousFill = null;
        string buttonname = "";
        public int c, r;
        // Point startPoint;
        int iteratorR, iteratorC;

        public Button DragSource { get; set; }
        public Image ImageDragSource { get; set; }

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

        private void btnMouseleftdown(object sender, MouseButtonEventArgs e)
        {


            Button TButton = sender as Button;
            buttonname = TButton.Name;
            string ganzeposition = buttonname.Substring(6, 3);
            r = int.Parse(ganzeposition.Substring(0, 1));
            c = int.Parse(ganzeposition.Substring(2, 1));
            GetTNPos(c, r);

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

            //imgx.Source = mw_parent.tnThumbnail;
            //lbtitle.Content = mw_parent.tnTitle;
            //lburl.Content = mw_parent.tnUrl;


            Image TImage = sender as Image;
            DragSource = TButton;
            ImageDragSource = imgx;

            DataObject dataObj = new DataObject();
            dataObj.SetData("Title", lbtitle.Content);
            dataObj.SetData("Image", imgx.Source);
            dataObj.SetData("Url", lburl.Content);

            lbtitle.Content = "";
            imgx.Source = null;
            lburl.Content = "";
            if (TButton != null && e.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(TButton, dataObj, DragDropEffects.Move);
            }
        }


        private void DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;


            string dataString = (string)e.Data.GetData(DataFormats.StringFormat);

            BrushConverter converter = new BrushConverter();
            if (converter.IsValid(dataString))
            {
                e.Effects = DragDropEffects.Copy | DragDropEffects.Move;
            }

        }

        private void DragEnter(object sender, DragEventArgs e)
        {
            Button TButton = sender as Button;
            if (TButton != null)
            {
                _previousFill = TButton.Background;

                //      if (e.Data.GetDataPresent(DataFormats.StringFormat))
                //    {
                string dataString = (string)e.Data.GetData(DataFormats.StringFormat);
                //  string ButtonName = TButton.Name;
                // string TitleN = 

                BrushConverter converter = new BrushConverter();
                if (converter.IsValid(dataString))
                {
                    Brush newFill = (Brush)converter.ConvertFromString(dataString);
                    TButton.Background = newFill;
                }
                //        }
            }
        }

        private void Item_DragLeave(object sender, DragEventArgs e)
        {

        }

        private void Item_Dropped(object sender, DragEventArgs e)
        {
            // var button = (Button)sender;
            Button TButton = sender as Button;
            buttonname = TButton.Name;
            string ganzeposition = buttonname.Substring(6, 3);
            r = int.Parse(ganzeposition.Substring(0, 1));
            c = int.Parse(ganzeposition.Substring(2, 1));
            GetTNPos(c, r);

            // Border TButton = (Border)sender;
            // Button TButton = sender as Button;
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

            if (TButton != null)
            {
                string dataString = (string)e.Data.GetData(DataFormats.StringFormat);

                imgx.Source = (ImageSource)e.Data.GetData("Image");
                lbtitle.Content = (String)e.Data.GetData("Title");
                lburl.Content = mw_parent.tnUrl;
            }
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

        private void MakeElementName(int xgridRow, int xgridCol)
        {
            imageElement = "Image";
            imageElement = imageElement + xgridRow.ToString() + "x" + xgridCol.ToString();

            // String Builder für Title
            titleElement = "Title";
            titleElement = titleElement + xgridRow.ToString() + "x" + xgridCol.ToString();
            // String Builder für Url
            UrlElement = "Url";
            UrlElement = UrlElement + xgridRow.ToString() + "x" + xgridCol.ToString();

            Image imgx = this.FindName(imageElement) as Image;
            Label lbtitle = this.FindName(titleElement) as Label;
            Label lburl = this.FindName(UrlElement) as Label;

            imgx.Source = mw_parent.tnThumbnail;
            lbtitle.Content = mw_parent.tnTitle;
            lburl.Content = mw_parent.tnUrl;

        }

        string TNfromPC, TNfromPCtoo;
        private void onAddfromPcClicked(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.OpenFileDialog opd = new System.Windows.Forms.OpenFileDialog())
            {
                if (opd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //  TNfromPC = fbd.;
                    TNfromPC = opd.FileName;
                }
            }

            using (System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //  TNfromPC = fbd.;
                    TNfromPCtoo = fbd.SelectedPath;
                }
            }

            TNfromPC = TNfromPC + TNfromPCtoo;

        }


        private void TSEFWindow_Closed(object sender, EventArgs e)
        {
            // MessageBox.Show("passt" + mw_parent.selectedRow + "x" + mw_parent.selectedColumn);
            //  MessageBox.Show("Something has happened" + selectedRow + "" + selectedColumn, "Parent");
            int nRow = 0;
            while (nRow == 3)
            {
                titleElement = "Title";
                titleElement = titleElement + "0x" + nRow.ToString();
                Label lbtitleX = this.FindName(titleElement) as Label;
                if (lbtitleX.Content.ToString() != "")
                {
                    nRow++;
                }
                else
                {
                    gridRow = nRow;
                }
            }






            if (iteratorC == 4)
            {
                iteratorC = iteratorC - 1;
            }
            titleElement = "Title";
            titleElement = titleElement + iteratorR.ToString() + "x" + iteratorC.ToString();
            Label lbtitle = this.FindName(titleElement) as Label;
            // State 1: Erstes ist frei, Rest ist frei
            // State 2: Erstes is frei, zweites nicht
            // State 3: Erstes is frei, rest nicht
            // State 4: mind. Zweites is frei, rest nicht

            //   while (lbtitle.Content.ToString() != "")

            // State 1: Erstes ist frei, Rest ist frei       
            if (lbtitle.Content.ToString() == "")
            {

                //this.selectedColumn = mw_parent.selectedColumn;
                //this.selectedRow = mw_parent.selectedRow;
                //this.tnTitle = mw_parent.tnTitle;
                //this.tnUrl = mw_parent.tnUrl;
                //this.tnThumbnail = mw_parent.tnThumbnail;
                if (lbtitle.Content.ToString() == "")
                    if (iteratorC == 0)
                    {
                        columnPos = 0;
                    }
                gridCol = iteratorC;
                iteratorC++;


            }
            // State 2: Erstes is frei, zweites nicht
            else
            {
                if (iteratorC <= 0)
                {

                }
                else
                {
                    iteratorC--;
                }

                titleElement = "Title";
                titleElement = titleElement + iteratorR.ToString() + "x" + iteratorC.ToString();
                lbtitle = this.FindName(titleElement) as Label;

                gridCol = iteratorC;
                gridRow = iteratorR;

                if (iteratorC == 4)
                {
                    iteratorC = iteratorC - iteratorC;
                    iteratorR++;
                }
            }

            //Image0x0.Source = mw_parent.tnThumbnail;
            //Title0x0.Content = mw_parent.tnTitle;
            //Url0x0.Content = mw_parent.tnUrl;

            // 
            if (gridRow < 2)
            {
                if (columnPos <= 4)
                {
                    if (columnPos <= 3)
                    {
                        MakeElementName(gridRow, gridCol);
                        columnPos = gridCol;
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
                            MakeElementName(gridRow, gridCol);
                            if (gridCol == 0)
                            {
                                gridCol++;
                            }
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
