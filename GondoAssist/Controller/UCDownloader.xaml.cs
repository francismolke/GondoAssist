using System;
using System.Collections.Generic;
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
using YoutubeExtractor;

namespace GondoAssist
{
    /// <summary>
    /// Interaktionslogik für UCDownloader.xaml
    /// </summary>
    public partial class UCDownloader : UserControl
    {
            
        public string link;
        List<VideoInfo> VInfos;
        // default speicherort
        public string DefDownloadPath { get; } = @"C:\Users\Agrre\Desktop\C#Test";


        public UCDownloader(string xlink)
        {
            link = xlink;
            InitializeComponent();
            if (xlink == "")
            {
                lbVideoName.Text = "hier link einfügen";
            }
            else { 
            cBLinks.Items.Clear();
            cBLinks.ItemsSource = FetchAvailableDownloadFormats(link);
            cBLinks.SelectedIndex = 0;
            }
        }
        // Überprüfe ob URL Valid ist
        private bool CheckUrlIsValid(string link)
        {
            return Uri.IsWellFormedUriString(link, UriKind.Absolute);
        }

        // Hole dir die Verfügbaren Download Formate
        private string[] FetchAvailableDownloadFormats(string link)
        {
            var videos = DownloadUrlResolver.GetDownloadUrls(link, false);

            string[] strVideos = new string[videos.Count()];
            VInfos = new List<VideoInfo>();

            int iterationCount = 0;
            foreach (var video in videos)
            {//Video = videoformat

                if (iterationCount == 0)
                    lbVideoName.Text = video.Title;
                strVideos[iterationCount++] = string.Format("Resolution: {0}, Format: {1}",
                        video.Resolution, video.VideoExtension);
                VInfos.Add(video);
            }
            return strVideos;
        }
        // Download-Prozess wird gestartet
        private void onDownloadClicked(object sender, RoutedEventArgs e)
        {

            var webClient = new WebClient();

            int indexToDw = cBLinks.SelectedIndex;
            string linkFullPathToDownload = String.Empty;
            if (string.IsNullOrEmpty(lbVideoName.Text))
            {
                linkFullPathToDownload = string.Format(@"{0}\{1}.{2}",
                 urlbox.Text, MakeValidFileName(VInfos[indexToDw].Title), VInfos[indexToDw].VideoExtension.Substring(1));
            }
            else
            {
                linkFullPathToDownload = string.Format(@"{0}\{1}.{2}",
                    urlbox.Text, MakeValidFileName(lbVideoName.Text), VInfos[indexToDw].VideoExtension.Substring(1));
            }

            webClient.DownloadProgressChanged += (s, ev) =>
            {
                progressBar.Value = ev.ProgressPercentage;
                lblPercent.Content = string.Format("{0}%", ev.ProgressPercentage.ToString());
                lblTotalDW.Content = string.Format("{0}MB's / {1}MB's", (ev.BytesReceived / 1024 / 1024).ToString("0.00"), (ev.TotalBytesToReceive / 1024 / 1024).ToString("0.00"));
            };
            webClient.DownloadFileCompleted += (s, ev) =>
            {


            };

            webClient.DownloadFileAsync(new Uri(VInfos[indexToDw].DownloadUrl), linkFullPathToDownload);
        }


        // Erstelle einen gültigen Dateinamen
        private object MakeValidFileName(string title)
        {
            var builder = new StringBuilder();
            var invalidChars = System.IO.Path.GetInvalidFileNameChars();
            foreach (char c in title)
            {
                if (!invalidChars.Contains(c))
                {
                    builder.Append(c);
                }
            }
            return builder.ToString();
        }
        // Speicherort wählen Button
        private void onSaveFolderClicked(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog() { Description = "Select your Path" })
            {
                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    urlbox.Text = fbd.SelectedPath;
                }
            }

            if (!CheckUrlIsValid(link))
            {
                System.Windows.MessageBox.Show(@"In-Valid Url");
            }
        }

        // Event bei Textänderungen beim Link
        private void onLinkTextChanged(object sender, RoutedEventArgs e)
        {
            cBLinks.IsEnabled = (true);
            if (lbVideoName.Text == "hier link einfügen" || lbVideoName.Text == "" || lbVideoName.Text == null)
            {
                cBLinks.IsEnabled = (false);
            }
            else
            { 
            cBLinks.Items.Clear();
            link = lbVideoName.Text;
            cBLinks.ItemsSource = FetchAvailableDownloadFormats(link);
            cBLinks.SelectedIndex = 0;
            }
        }
    }
}
