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
using System.Windows.Shapes;
using YoutubeExtractor;
using System.IO;
using System.Text.RegularExpressions;

namespace GondoAssist
{
    /// <summary>
    /// Interaktionslogik für TestDownloader.xaml
    /// </summary>
    public partial class TestDownloader : UserControl
    {
        public string link = "https://youtu.be/11HvU-LSi_Q";
        List<VideoInfo> VInfos;
        //public string DefDownloadPath { get; } = @"C:\Users\Agrre\Desktop\C#Test";


        public TestDownloader()
        {
            InitializeComponent();

        }

        //es war static
        private static void DownloadVideo(IEnumerable<VideoInfo> videoInfos)
        {    /*
             * Select the first .mp4 video with 360p resolution
             */
            VideoInfo video = videoInfos.First(info => info.VideoType == VideoType.Mp4 && info.Resolution == 1080);
            //decryption
            if (video.RequiresDecryption)
            {
                DownloadUrlResolver.DecryptDownloadUrl(video);
            }
            /*
             * Create the video downloader.
             * The first argument is the video to download.
             * The second argument is the path to save the video file.
             */
            var videoDownloader = new VideoDownloader(video,
                 System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                 RemoveIllegalPathCharacters(video.Title) + video.VideoExtension));

            //videoDownloader.DownloadProgressChanged += (s, ev) => 
            //{
            //    progressBar.Value = ev.ProgressPercentage;
            //    lblPercent.Content = string.Format("{0}%", ev.ProgressPercentage.ToString());
            //    lblTotalDW.Content = string.Format("{0}MB's / {1}MB's", (ev.BytesReceived / 1024 / 1024).ToString("0.00"), (ev.TotalBytesToReceive / 1024 / 1024).ToString("0.00"));
            //};

            /*
             * Execute the video downloader.
             * For GUI applications note, that this method runs synchronously.
             */
            videoDownloader.Execute();

        }

        private static string RemoveIllegalPathCharacters(string path)
        {
            string regexSearch = new string(System.IO.Path.GetInvalidFileNameChars()) + new string(System.IO.Path.GetInvalidPathChars());
            var r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            return r.Replace(path, "");
        }

        // Download-Prozess wird gestartet
        private void onDownloadClicked(object sender, RoutedEventArgs e)
        {
            IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(link, false);

            //DownloadAudio(videoInfos);
            DownloadVideo(videoInfos);
        }

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

        // Überprüfe ob URL Valid ist
        private bool CheckUrlIsValid(string link)
        {
            return Uri.IsWellFormedUriString(link, UriKind.Absolute);
        }

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
    }
}
