using GondoAssist.Klassen;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GondoAssist
{
    /// <summary>
    /// Interaktionslogik für Uploader_Youtube.xaml
    /// </summary>
    public partial class Uploader_Youtube : UserControl
    {
        string xmlfilepath;
        string speicherort;
        string Titel, Beschreibung, Tags, Pfad;
        string privacystatus;
        string fileName = "Youtube_VideoDescription.txt";
        MainWindow mw;

        public Uploader_Youtube(MainWindow mw)
        {
            this.mw = mw;
            string path = Path.Combine(Environment.CurrentDirectory, @"User\", fileName);      
            InitializeComponent();
            DescriptionBox.Text = File.ReadAllText(path);
        }

        private void onSelectFileButtonClicked(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
            {

                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    pathbox.Text = ofd.FileName;
                }
            }
        }

        private void onSelectFolderTSClicked(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog() { Description = "Select your Path" })
            {
                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    speicherort = fbd.SelectedPath;
                }
            }
        }

        private void onXMLClicked(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
            {
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    xmlfilepath = ofd.FileName;
                }
            }

        }

        private void onTimestampCreatedClicked(object sender, RoutedEventArgs e)
        {
            Timestamp.GetXMLInformation(speicherort, xmlfilepath);
        }

        private void onListboxItemSelected(object sender, SelectionChangedEventArgs e)
        {
            privacystatus = Privacy.SelectedValue.ToString();
            if (privacystatus == "System.Windows.Controls.ListBoxItem: Privat")
            {
                privacystatus = "private";
            }
            else if (privacystatus == "System.Windows.Controls.ListBoxItem: Nicht gelistet")
            {
                privacystatus = "unlisted";
            }
            else if (privacystatus == "System.Windows.Controls.ListBoxItem: Öffentlich")
            {
                privacystatus = "public";
            }
            else
            {
                privacystatus = "unlisted";
            }

            TagBox.Text = privacystatus;

        }

        private async Task Run()
        {
            // Die Login-Daten für den Youtube-Kanal werden hier behandelt
            UserCredential credential;
            using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    // This OAuth 2.0 access scope allows an application to upload files to the
                    // authenticated user's YouTube channel, but doesn't allow other types of access.
                    new[] { YouTubeService.Scope.YoutubeUpload },
                    "user",
                    CancellationToken.None
                );
            }
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
            });

            // hier werden die Daten für Titel, Beschreibung etc übergeben
            var video = new Video();
            video.Snippet = new VideoSnippet();
            video.Snippet.Title = Titel;
            video.Snippet.Description = Beschreibung;
            video.Snippet.Tags = new string[] { Tags };
            video.Snippet.CategoryId = "22"; // See https://developers.google.com/youtube/v3/docs/videoCategories/list
            video.Status = new VideoStatus();
            video.Status.PrivacyStatus = privacystatus; // "unlisted"; // or "private" or "public"
            var filePath = Pfad; // Replace with path to actual movie file.

            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                var videosInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");
                videosInsertRequest.ProgressChanged += videosInsertRequest_ProgressChanged;
                videosInsertRequest.ResponseReceived += videosInsertRequest_ResponseReceived;

                videosInsertRequest.UploadAsync().Wait();
            }
        }

        void videosInsertRequest_ProgressChanged(Google.Apis.Upload.IUploadProgress progress)
        {
            switch (progress.Status)
            {
                case UploadStatus.Uploading:
                    MessageBox.Show("Uploading...");
                    break;

                case UploadStatus.Failed:
                    MessageBox.Show("An error prevented the upload from completing.\n{0}", progress.ToString());
                    break;
            }
        }

        void videosInsertRequest_ResponseReceived(Video video)
        {
            MessageBox.Show("Upload finished");
        }


        private void onUploadClicked(object sender, RoutedEventArgs e)
        {
            Titel = titletb.Text;
            Beschreibung = DescriptionBox.Text;
            Tags = TagBox.Text;
            Pfad = pathbox.Text;
            try
            {
              
                Run().Wait();
            }
            catch (AggregateException ex)
            {
                foreach (var be in ex.InnerExceptions)
                {
                    throw new System.ArgumentException("errormessege:" + be.Message);
                }
            }

        }
    }
}
