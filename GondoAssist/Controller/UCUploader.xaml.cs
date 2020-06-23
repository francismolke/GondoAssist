using GondoAssist.Klassen;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using HtmlAgilityPack;
//using NUnit.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WordPressPCL;
using WordPressPCL.Models;
using WordPressPCL.Tests.Selfhosted.Utility;

namespace GondoAssist
{
    /// <summary>
    /// Interaktionslogik für UCUploader.xaml
    /// </summary>
    public partial class UCUploader : UserControl
    {
        string xmlfilepath;
        string speicherort;
        string Titel, Beschreibung, Tags, Pfad;
        string privacystatus;
        string GDdatentyp;
        string _filePath;
        private static WordPressClient _clientAuth;
        private static WordPressClient _client;
        BackgroundWorker backgroundWorker1 = new BackgroundWorker();
        //Window IGPB = new Window();

        public UCUploader()
        {
            InitializeComponent();
            DataContext = this;
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;

            //IGPB.Height = 300;
            //IGPB.Width = 400;
            //IGPB.Name = "IGProgressBar";
            //IGPB.WindowStartupLocation = WindowStartupLocation.CenterOwner;




        }
        [ClassInitialize]
        public static async Task Init(TestContext testContext)
        {
            _client = ClientHelper.GetWordPressClient();
            _clientAuth = await ClientHelper.GetAuthenticatedWordPressClient();
        }

        #region Der Upload-Prozess für Youtube        
        // Der Upload-Prozess für Youtube        
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
        // Status des Uploads
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
        // Upload Finish
        void videosInsertRequest_ResponseReceived(Video video)
        {
            MessageBox.Show("Upload finished");
        }
        // Buttonevent für Datei Auswahl
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
        // Youtube Privacy Listbox-Event
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
        // Buttonevent für Upload starten
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
        #endregion
        #region Buttonevent für das Uploaden auf der Webseite
        // Buttonevent für das Uploaden auf der Webseite
        private async void onPostClicked(object sender, RoutedEventArgs e)
        {
            await CreatePost(TNSavePath, TNSaveFile, wtitletb.Text, wDescriptionBox.Text);
        }


        string TNSavePath, TNSaveFile;
        public async Task Media_Create()
        {

            //  var path = Directory.GetCurrentDirectory() + "\\Assets\\cat.jpg";
            var path = TNSaveFile;
            Debug.WriteLine(File.Exists(path));
            Stream s = File.OpenRead(path);
            var mediaitem = await _clientAuth.Media.Create(s, TNSavePath);
            Assert.IsNotNull(mediaitem);
        }

        private void onThumbnailOpenClicked(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.OpenFileDialog fbd = new System.Windows.Forms.OpenFileDialog())
            {
                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    TNSavePath = fbd.FileName;
                    TNSaveFile = System.IO.Path.GetFileName(TNSavePath);
                }
            }
        }
        // Der Prozzes für das erstellen eines Beitrags auf der Webseite

        private static async Task CreatePost(string path, string file, string title, string content)
        {
            try
            {
                WordPressClient client = await GetClient();




                if (await client.IsValidJWToken())
                {

                    //         var path = Directory.GetCurrentDirectory() + "\\Assets\\cat.jpg";
                    ////         var path = Directory.GetCurrentDirectory() + "\\Assets\\cat.jpg";
                    //var path = TNSaveFile;

                    //Debug.WriteLine(File.Exists(path));

                    //var mediaitem = await client.Media.Create(path, "cat.jpg");
                    //Assert.IsNotNull(mediaitem);


                    Debug.WriteLine(File.Exists(path));

                    var mediaitem = await client.Media.Create(path, file);

                    //var mediaitem = await _clientAuth.Media.Create(path, "cat.jpg");
                    Assert.IsNotNull(mediaitem);

                    var post = new Post
                    {
                        Title = new Title(title),
                        Content = new Content(content),
                        FeaturedMedia = mediaitem.Id

                    };

                    await client.Posts.Create(post);
                    MessageBoxResult result = MessageBox.Show("Upload finished");


                }
            }
            catch (Exception e)
            {
                throw new Exception("ErrorMessage: " + e.Message);
            }

        }
        // Hier werden die Login-Daten von Wordpress abgehandelt
        private static async Task<WordPressClient> GetClient()
        {
            //JWT authentication
            var client = new WordPressClient("http://www.slamdank31.com/wp-json/");
            client.AuthMethod = AuthMethod.JWT;
            await client.RequestJWToken("Arschloch1", "Slimtwix13");
            return client;
        }
        #endregion
        #region Initialisierung der Google Drive Variablen
        // Initialisierung der Google Drive Variablen
        private static string[] Scopes = { DriveService.Scope.Drive };
        private static string ApplicationName = "GoogleDriveAPIStart";
        private static string FolderId = "1lgB8zg1AqOBN99R_l6s0LsQTM4Br3WbE";
        private static string _fileName = "testFile";
        //    private static string _filePath = @"C:\Users\Agrre\Desktop\Neuer Ordner\hehe.rar";
        //  private static string _contentType = GDdatentyp;
        //"application/zip";
        //image/jpeg
        //video/mp4
        //text/csv

        //video = video/mp4, bild = image/jpeg, zip = application/zip

        // Listbox Event für die Auswahl des Datentypes für Google Drive
        private void onGDLBISelected(object sender, SelectionChangedEventArgs e)
        {
            GDdatentyp = LBDatatype.SelectedValue.ToString();
            if (GDdatentyp == "System.Windows.Controls.ListBoxItem: Video")
            {
                GDdatentyp = "video/mp4";
            }
            else if (GDdatentyp == "System.Windows.Controls.ListBoxItem: Bild")
            {
                GDdatentyp = "image/jpeg";
            }
            else if (GDdatentyp == "System.Windows.Controls.ListBoxItem: ZIP")
            {
                GDdatentyp = "application/zip";
            }
            else
            {
                GDdatentyp = "unlisted";
            }

            gDescriptionBox.Text = GDdatentyp;
        }
        // Button event für Datei auswahl für Google Drive 
        private void onGDSelectFile(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _filePath = ofd.FileName;
                }
        }
        // Hier wird für Google Drive die Status-Meldungen abgehandelt
        private void onGPostClicked(object sender, RoutedEventArgs e)
        {
            UploadControl.Content = "Uploading....please wait";

            try
            {
                //     Console.WriteLine("Create creds");
                UserCredential credential = GetUserCredential();

                //      Console.WriteLine("get Serivce");
                DriveService service = GetDriveService(credential);
                _fileName = gtitletb.Text;
                if (GDdatentyp == "video/mp4")
                {
                    _fileName += ".mp4";
                    gDescriptionBox.Text = GDdatentyp;
                }
                else if (GDdatentyp == "image/jpeg")
                {
                    _fileName += ".jpg";
                    gDescriptionBox.Text = GDdatentyp;
                }

                else if (GDdatentyp == "application/zip")
                {
                    _fileName += ".zip";
                    gDescriptionBox.Text = GDdatentyp;
                }
                else
                {
                    gDescriptionBox.Text = "Error";
                    // ErrorMessage
                }


                //      Console.WriteLine("Uploading...");
                UploadFileToDrive(service, _fileName, _filePath, GDdatentyp);
                UploadControl.Content = "Upload complete";
            }

            catch (AggregateException ex)
            {
                foreach (var de in ex.InnerExceptions)
                {
                    throw new AggregateException("ErrorMessage:" + de.Message);
                }
            }
        }
        // Erhält die Google Services
        private static DriveService GetDriveService(UserCredential credential)
        {
            return new DriveService(
                new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName
                });
        }
        // Die Google Drive Login-Daten werden hier abgehandelt
        private static UserCredential GetUserCredential()
        {

            using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                string createPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                createPath = System.IO.Path.Combine(createPath, "driveApiCredentials", "drive-credentials.json");

                return GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "User",
                    CancellationToken.None,
                    new FileDataStore(createPath, true)).Result;

            }

        }
        // Hier wird der Google Drive Upload Prozess abgehandelt
        private static string UploadFileToDrive(DriveService service, string fileName, string filePath, string contentType)
        {
            var fileMetaData = new Google.Apis.Drive.v3.Data.File();
            fileMetaData.Name = fileName;
            fileMetaData.Parents = new List<string> { FolderId };
            FilesResource.CreateMediaUpload request;

            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                request = service.Files.Create(fileMetaData, stream, contentType);
                request.Fields = "id";
                request.Upload();
            }
            var file = request.ResponseBody;
            return file.Id;
        }

        private void collectVideos(object sender, RoutedEventArgs e)
        {
            string sourcePath = @"C:\Users\Agrre\Desktop\JdownloadSlamdan";
            string targetPath = @"C:\Users\Agrre\Desktop\" + EpisodeNumber.Text;
            string directoryName;
            string destDirectory;
            string fileName;
            string destFile;
            Directory.CreateDirectory(targetPath);
            // check if directory exists
            if (Directory.Exists(sourcePath))
            {
                Directory.CreateDirectory(targetPath);
                // get directories into arrays
                string[] directory = Directory.GetDirectories(sourcePath);

                foreach (string s in directory)
                {
                    // get files firom directories into arrays
                    string[] files = Directory.GetFiles(s);

                    directoryName = System.IO.Path.GetDirectoryName(s);
                    foreach (string c in files)
                    {
                        // move files to destination
                        fileName = System.IO.Path.GetFileName(c);
                        //destDirectory = JD+filename
                        destDirectory = System.IO.Path.Combine(s, fileName);
                        // der neue pfad und name für die datei wo sie gespeichert werden soll

                        // combine targetpath und filename
                        // destFile = Desktop+Filename
                        destFile = System.IO.Path.Combine(targetPath, fileName);
                        File.Move(destDirectory, destFile);
                    }


                }
            }
        }




        #endregion

        #region  Timestamp Speicherort

        // Timestamp Speicherort
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
        // Timestamp Datei auswahl
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

        // Button Event für das Abrufen der Timestamp Methode
        public void onTimestampCreatedClicked(object sender, RoutedEventArgs e)
        {
            Timestamp.GetXMLInformation(speicherort, xmlfilepath);
        }
        #endregion

        string sourcepath = @"C:\Users\Agrre\Desktop\InstagramProfileList.txt";
        string savepath = @"C:\Users\Agrre\Desktop\";


        private void DateTimeCheck()
        {


            int counter = 1;

            DateTime dateTimeNow = DateTime.Now;
            DateTime lastThuesday = DateTime.Now.AddDays(-1);
            DateTime lastFriday = DateTime.Now.AddDays(-1);
            DateTime insertDate = (DateTime)IGDatePicker.SelectedDate;
            DateTime suggestedDate;
            //  insertDate = dpSelectedDate;

            if (insertDate == null)
            {
                if (dateTimeNow.DayOfWeek == DayOfWeek.Friday)
                {


                    while (lastThuesday.DayOfWeek != DayOfWeek.Friday)
                    {
                        lastThuesday = lastThuesday.AddDays(-1);
                        counter++;
                    }
                    TimeSpan diffDays = dateTimeNow.Subtract(lastThuesday);
                    suggestedDate = dateTimeNow - diffDays;
                }
                else
                {


                    while (lastFriday.DayOfWeek != DayOfWeek.Friday)
                    {
                        lastFriday = lastFriday.AddDays(-1);
                        counter++;
                    }
                    TimeSpan diffDays = dateTimeNow.Subtract(lastFriday);
                    suggestedDate = dateTimeNow - diffDays;

                }
                dpSelectedDate.SelectedDate = suggestedDate;
                GetInstagramList(suggestedDate);
            }


            GetInstagramList(insertDate);


        }





        public void GetHTMLInfo(List<string> profileList, DateTime suggestedDate)
        {
            int counter = 0;
            File.WriteAllText(savepath + "\\Quellen.txt", String.Empty);
            var service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            IWebDriver driver = new ChromeDriver(service, new ChromeOptions());

            driver.Manage().Window.Minimize();
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(20);

            //


            //IGPB.Show();
            //
            ProgressBar IGPBar = new ProgressBar()
            {
                Minimum = 0,
                Maximum = profileList.Count,
            }; ;
            //IGPB.Content = IGPBar;

            while (counter < profileList.Count)
            {

                backgroundWorker1.DoWork += backgroundWorker1_DoWork;
                backgroundWorker1.ProgressChanged += worker_ProgressChanged;
                //progressBarIG
                //progressBarIG.Minimum = 1;
                //progressBarIG.Maximum = profileList.Count;
                //progressBarIG.Value = 1;
                //  BackgroundWorker worker = new BackgroundWorker();
                //  worker.WorkerReportsProgress = true;
                //  worker.DoWork += worker_DoWork;
                // https://www.wpf-tutorial.com/misc-controls/the-progressbar-control/
                driver.Url = profileList.ElementAt(counter);
                var html = driver.PageSource;
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);


                #region Select 1 Node
                //var node = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='v1Nh3 kIKUG  _bz0w']//a[@href]");

                //if (node == null)
                //{
                //    label1.Content = "error";
                //    label2.Content = "error";
                //    Thread.Sleep(3000);
                //    driver.Quit();
                //}
                //else
                //{
                //    label1.Content = node.Attributes["href"].Value;
                //    label2.Content = node.OuterHtml;
                //    Thread.Sleep(3000);
                //    driver.Quit();
                //}


                //Select all nodes (while timeoutspan and scroll-range)
                #endregion

                var node1 = htmlDoc.DocumentNode.SelectNodes("//div[@class='v1Nh3 kIKUG  _bz0w']//a[@href]");
                if (node1 != null)
                {

                    using (StreamWriter writer = new StreamWriter(savepath + "\\Quellen.txt", true, Encoding.UTF8))
                    {

                        string link = "";
                        DateTime returnValue;
                        foreach (var item in node1)
                        {
                            link = "https://www.instagram.com" + item.Attributes["href"].Value;
                            returnValue = DateTimeExpress(link, driver);
                            // DateTime dt1 = new DateTime(2019, 8, 9, 20, 0, 0);
                            if (returnValue > suggestedDate)
                                writer.Write("https://www.instagram.com" + item.Attributes["href"].Value + "\r\n");
                            else
                            { }
                            // writer.Write(profileList);
                        }
                        writer.Close();
                    }

                    //  Thread.Sleep(3000);
                    //driver.Quit();

                }
                else
                {
                    //   Thread.Sleep(3000);
                    //  driver.Quit();

                }
                //      progressBarIG.Value += counter;
                counter++;
            }
            driver.Quit();
        }




        int profilecount;

        private int GetInstagramList(DateTime suggestedDate)
        {
            List<string> profileList = new List<string>();
            using (StreamReader filereader = new StreamReader(sourcepath))
            {
                foreach (string line in File.ReadLines(sourcepath, Encoding.UTF8))
                {
                    profileList.Add(line);
                }
                // startBackGroundWOrker(profileList.Count, profileList, suggestedDate);


                GetHTMLInfo(profileList, suggestedDate);
            }
            return profilecount = profileList.Count;
        }

        private void startBackGroundWOrker(int count, List<string> profileList, DateTime suggestedDate)
        {

            if (backgroundWorker1.IsBusy != true)
            {
                backgroundWorker1.RunWorkerAsync();
            }

            //using (StreamReader filereader = new StreamReader(sourcepath))
            //{
            //    foreach (string line in File.ReadLines(sourcepath, Encoding.UTF8))
            //    {
            //        profilecount++;
            //    }

            // open dialog
            // mit pg hier erstellen dynamisch
            GetHTMLInfo(profileList, suggestedDate);
            //  ShowProgressHandler(count, profileList, suggestedDate);
            //}
        }


        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            for (int i = 0; i <= profilecount; i++)
            {
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    if (i != 0)
                    {
                        int percentage = profilecount / i;
                        worker.ReportProgress(percentage);
                    }
                    //
                }
            }
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

            pbStatus.Value = e.ProgressPercentage;
        }


        private void cancelBackGroundWorker()
        {

        }


        //            var progressbar = new ProgressBar()

        ////add it to the form
        //            uploadForm.Controls.Add(progressbar);


        private void ShowProgressHandler(int count, List<string> profileList, DateTime suggestedDate)
        {
            Window IGPB = new Window();
            IGPB.Height = 300;
            IGPB.Width = 400;
            IGPB.Name = "IGProgressBar";
            IGPB.WindowStartupLocation = WindowStartupLocation.CenterOwner;


            ProgressBar IGPBar = new ProgressBar()
            {
                Minimum = 0,
                Maximum = count,
            }; ;
            IGPB.Content = IGPBar;

            IGPB.Show();
            GetHTMLInfo(profileList, suggestedDate);

            //for (int i = 0; i < count; i++)
            //{
            //    IGPBar.Value++;
            //}
        }


        string TimeStopRaw;

        private void onCreateIGListClicked(object sender, RoutedEventArgs e)
        {

            DateTimeCheck();

        }

        private DateTime DateTimeExpress(string Link, IWebDriver driver)
        {

            string link = Link;

            driver.Url = link;

            var html = driver.PageSource;
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var linkdate2 = htmlDoc.DocumentNode.SelectNodes("//time[@class='_1o9PC Nzb55'][@datetime]");

            foreach (var item in linkdate2)
            {
                TimeStopRaw = item.Attributes["datetime"].Value;
            }
            string date, monthday, minsec;
            string TimeStamp;
            int year, month, day;
            int hour, minute, seconds;
            //22-07-2019 & 18:02:03
            // 18:02:03
            #region 2019/07/22 Uhrzeit Format
            ///2019-07-22 Uhrzeit Format
            /// Entweder: 2008-10-01T17:04:32
            /// oder  2008-10-01 17:04:32Z
            ///
            int TExtPos = TimeStopRaw.LastIndexOf('T');
            // if (TExtPos >= 0)            
            date = TimeStopRaw.Substring(0, TExtPos);
            // date = date.Replace('-', '.');
            // 2019-07-22
            int StrichPos = date.IndexOf('-');
            year = int.Parse(date.Substring(0, StrichPos));
            //    07-22
            monthday = date.Substring(date.IndexOf('-') + 1);
            StrichPos = monthday.IndexOf('-');
            month = int.Parse(monthday.Substring(0, StrichPos));
            //  -22
            day = int.Parse(monthday.Substring(date.IndexOf('-') - 1).Trim());


            string time = string.Format(TimeStopRaw.Substring(TimeStopRaw.IndexOf('T') + 1).Trim());

            int fileExtPos = time.LastIndexOf(".");
            // if (fileExtPos >= 0)
            time = time.Substring(0, fileExtPos);
            // 18:02:03
            fileExtPos = time.IndexOf(':');
            hour = int.Parse(time.Substring(0, fileExtPos));
            // 02:03
            minsec = time.Substring(time.IndexOf(':') + 1);
            fileExtPos = minsec.IndexOf(':');
            minute = int.Parse(minsec.Substring(0, fileExtPos));
            // :03
            seconds = int.Parse(minsec.Substring(time.IndexOf(':') + 1).Trim());
            //     xday = minsec.Substring(time.IndexOf(':') +1).Trim();

            #endregion

            //  DateTime parseDate = DateTime.Parse(xdeq);
            TimeStamp = date + " " + time + "" + year + "" + month + "" + day + "" + hour + "" + minute + "" + seconds;
            DateTime dt1 = new DateTime(year, month, day, hour, minute, seconds);

            return dt1;
        }

        private void GetVideoTimeDate()
        {
            IWebDriver driver = new ChromeDriver();

            driver.Url = "https://www.instagram.com/p/B0Om-xilkwu/";
            var html = driver.PageSource;
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);


            var linkdate = htmlDoc.DocumentNode.SelectNodes("//time[@class='_1o9PC Nzb55'][@datetime]");

            using (StreamWriter writer = new StreamWriter(savepath + "\\TestTime.txt", true, Encoding.UTF8))
            {
                foreach (var item in linkdate)
                {

                    writer.Write(item.Attributes["datetime"].Value + "\r\n");

                }
            }
            driver.Quit();
        }
        private void StopWatchTime()
        {
            IWebDriver iwd = new ChromeDriver();
            iwd.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(20);
            iwd.Url = "https://www.instagram.com/retry.mp4/";
            var html = iwd.PageSource;
            Stopwatch watch = null;
            watch = Stopwatch.StartNew();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var node1 = htmlDoc.DocumentNode.SelectNodes("//div[@class='v1Nh3 kIKUG  _bz0w']//a[@href]");
            watch.Stop();

            if (node1 != null)
            {
                string messageBoxText = "Es hat geklappt, Zeit: " + watch.ElapsedMilliseconds;
                string caption = "Word Processor";
                MessageBoxButton button = MessageBoxButton.YesNoCancel;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBox.Show(messageBoxText, caption, button, icon);

            }
            else
            {
                string messageBoxText = "Es hat nicht geklappt, Zeit: " + watch.ElapsedMilliseconds;
                string caption = "Word Processor";
                MessageBoxButton button = MessageBoxButton.YesNoCancel;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBox.Show(messageBoxText, caption, button, icon);
            }
        }

        private void onIGSavePathClicked(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog() { Description = "Select your Path" })
            {
                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    speicherort = fbd.SelectedPath;
                }
            }
        }


    }
}



//int profilecount = 0;

//private void CountProfileList()
//{


//    using (StreamReader filereader = new StreamReader(sourcepath))
//    {
//        foreach (string line in File.ReadLines(sourcepath, Encoding.UTF8))
//        {
//            profilecount++
//                }
//        // open dialog
//        // mit pg hier erstellen dynamisch
//        PB_Renderer(profilecount)
//            }
//}

//private void PB_Renderer(count)
//{

//    for (int i = 0; i < count; i++)
//    {
//        InstagramCheckPB.Value++;
//    }
//}


//    <Grid Margin = "20" >
//        < ProgressBar Name="InstagramCheckPB" Minimum="0" Value="75" />
//    </Grid>