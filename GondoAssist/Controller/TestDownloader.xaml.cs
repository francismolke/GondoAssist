using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using YoutubeExtractor;

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
        #region YoutubeDownloader
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
                    if (video.Title == "")
                    { }
                    else
                        lbVideoName.Text = video.Title;


                strVideos[iterationCount++] = string.Format("Resolution: {0}, Format: {1}",
                        video.Resolution, video.VideoExtension);
                VInfos.Add(video);
            }
            return strVideos;
        }

        #endregion

        #region IGDownloader

        public void IGDL(string downloadLink, string savepath)
        {
            try
            {

                ChromeOptions options = new ChromeOptions();
                options.AddArgument("--headless");
                var service = ChromeDriverService.CreateDefaultService();
                service.HideCommandPromptWindow = true;
                IWebDriver driver = new ChromeDriver(service, options);
                driver.Url = downloadLink;
                string fetchedDLLink = DownloadLinkExpress(driver);
                string creatorName = GetProfileName(driver);
                string creatorLink = MakeLinkName(downloadLink);
                string filename = creatorName + " - " + creatorLink;
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFile(fetchedDLLink, savepath + "\\" + filename + ".mp4");
                }
                MessageBox.Show("Download abgeschlossen");
            }
            catch (Exception ex)
            {
                using (StreamWriter writer = new StreamWriter("\\DownloadError.txt", true, Encoding.UTF8))

                {
                    writer.Write(ex + " und " + ex.Message);
                }

            }
        }

        #endregion
        string savepath;
        private void onIGDownloadClicked(object sender, RoutedEventArgs e)
        {
            string downloadLink = lbVidseoName.Text;
            savepath = urlbox.Text;
            IGDL(downloadLink, savepath);

        }



        private string GetProfileName(IWebDriver driver)
        {
            string profilename = "";
            if (IsElementPresent(By.CssSelector("a.sqdOP.yWX7d._8A5w5.ZIAjV"), driver))
            {
                profilename = driver.FindElement(By.CssSelector("a.sqdOP.yWX7d._8A5w5.ZIAjV")).Text;

            }
            return profilename;
        }

        private string MakeLinkName(string creatorlink)
        {
            // string neuerstring = v.Substring(v.LastIndexOf(@"/") - 11).Trim();
            string neuerstring = creatorlink.Substring(28).Trim();
            neuerstring = neuerstring.Remove(11).Trim();
            return neuerstring;
        }


        private string DownloadLinkExpress(IWebDriver driver)
        {
            string downloadLink = "";
            if (IsElementPresent(By.XPath("//meta[@property='og:video']"), driver))
            {
                downloadLink = driver.FindElement(By.XPath("//meta[@property='og:video']")).GetAttribute("content");

            }
            else if (IsElementPresent(By.XPath("//video[@class='tWeCl']"), driver))
            {
                downloadLink = driver.FindElement(By.XPath("//video[@class='tWeCl']")).GetAttribute("src");

            }
            return downloadLink;
        }

        private bool IsElementPresent(By by, IWebDriver driver)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        private void onOpenDownloadFolderClicked(object sender, RoutedEventArgs e)
        {
            savepath = urlbox.Text;

            if (savepath != "" && savepath != null)
            {
            Process.Start(savepath);

            }
        }
    }
}
