using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GondoAssist
{
    /// <summary>
    /// Interaktionslogik für InstagramGrabber.xaml
    /// </summary>
    public partial class InstagramGrabber : UserControl
    {

        BackgroundWorker backgroundWorker1 = new BackgroundWorker();
        string TNSavePath, TNSaveFile;
        string speicherort;
        // ÄNDERN
        // string savepath = @"C:\Users\E\Desktop\";
        //string sourcePath = @"C:\Users\Agrre\Desktop\JdownloadSlamdan";
        string sourcepath = "InstagramProfileList.txt";
        string TimeStopRaw;
        public InstagramGrabber()
        {
            InitializeComponent();
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


        private void onCreateIGListClicked(object sender, RoutedEventArgs e)
        {
            DateTimeCheck();
        }

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
                //dpSelectedDate.SelectedDate = suggestedDate;
                GetInstagramList(suggestedDate);
            }


            GetInstagramList(insertDate);


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

        public void GetHTMLInfo(List<string> profileList, DateTime suggestedDate)
        {
            string error = "";
            int counter = 0;
            File.WriteAllText(speicherort + "\\Quellen.txt", String.Empty);
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
            try
            {
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


                    // zweite
                    

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
                    var node2 = htmlDoc.DocumentNode.SelectNodes("//div[@class='vcOH2']");
                    if (node1 != null)
                    {

                        using (StreamWriter writer = new StreamWriter(speicherort + "\\Quellen.txt", true, Encoding.UTF8))
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

                        using (StreamWriter likeAbilityWriter = new StreamWriter(speicherort + "\\Quellen_Likeability.txt", true, Encoding.UTF8))
                        {

                            string link = "";
                            double returnValue;
                            DateTime returnValueDate;

                            foreach (var item in node1)
                            {
                                link = "https://www.instagram.com" + item.Attributes["href"].Value;
                                returnValue = DateTimeExpressForLikeability(link, driver);
                                returnValueDate = DateTimeExpress(link, driver);
                                // DateTime dt1 = new DateTime(2019, 8, 9, 20, 0, 0);
                                if (returnValueDate > suggestedDate)
                                likeAbilityWriter.Write(item.Attributes["href"].Value + returnValue +  "\r\n");
                                else
                                { }
                                // writer.Write(profileList);
                            }
                            likeAbilityWriter.Close();
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
            }
            catch (Exception e)
            {
                using (StreamWriter writer = new StreamWriter(speicherort + "\\QuellenError.txt", true, Encoding.UTF8))

                {
                    writer.Write(e + error);
                }
            }
            driver.Quit();
        }
        private double DateTimeExpressForLikeability(string Link, IWebDriver driver)
        {

            string link = Link;

            driver.Url = link;




            var html = driver.PageSource;
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var linkdate2 = htmlDoc.DocumentNode.SelectNodes("//time[@class='_1o9PC Nzb55'][@datetime]");
            var linkdate3 = htmlDoc.DocumentNode.SelectNodes("//div[@class='QhbhU'][@datetime]");

            foreach (var item in linkdate2)
            {
                TimeStopRaw = item.Attributes["datetime"].Value;
            }

            var aufrufeRoh = driver.FindElement(By.ClassName("vcOH2")).Text;
            driver.FindElement(By.ClassName("vcOH2")).Click();
            var likesRoh = driver.FindElement(By.ClassName("vJRqr")).Text;
            var aufruf = aufrufeRoh.Split(' ').First();
            var likes = likesRoh.Substring(likesRoh.IndexOf(' ') + 1).Trim();
            likes = likes.Split(' ').First();
            double summe = double.Parse(likes, NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture("en-GB")) / double.Parse(aufruf, NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture("en-GB"));
            summe = summe * 100;
            summe = Math.Round(summe, 2);

            return summe;
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

        string sourcePath;

        private void onIGOpenPathClicked(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog() { Description = "Select your Path" })
            {
                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    sourcePath = fbd.SelectedPath;
                }
            }
        }

        private void collectVideos(object sender, RoutedEventArgs e)
        {
            //string sourcePath = @"C:\Users\E\Desktop\JdownloadSlamdan";
            if (igtitle.Text == "")
            {
                igtitle.Text = "Heute";
            }
            string targetPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\" + igtitle.Text;
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

        //private async void onPostClicked(object sender, RoutedEventArgs e)
        //{
        //    await CreatePost(TNSavePath, TNSaveFile, wtitletb.Text, wDescriptionBox.Text);
        //}


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




    }
}
