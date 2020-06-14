using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace GondoAssist
{
    /// <summary>
    /// Interaktionslogik für InstagramGrabber.xaml
    /// </summary>
    public partial class InstagramGrabber : UserControl
    {

        BackgroundWorker backgroundWorker1 = new BackgroundWorker();
        // string TNSavePath, TNSaveFile;
        string speicherort, speicherortQuellenLikeability;
        string targetPath;
        string sourcePath, destinationPath;
        bool isBotProfileBlocked = false;
        bool botProfileIsActive = false;
        bool isOwnProfileIsActive = false;
        string IGLogin, IGPW = "";
        // ÄNDERN
        // string savepath = @"C:\Users\E\Desktop\";
        //string sourcePath = @"C:\Users\Agrre\Desktop\JdownloadSlamdan";
        string sourcepath = "InstagramProfileList.txt";
        string JDSettingsPath = "JDownloaderLocation.txt";
        string TimeStopRaw;
        public InstagramGrabber()
        {
            InitializeComponent();
            CheckIfJDownloaderPathIsSaved();
            CheckIfLoginIsSaved();
        }

        private void CheckIfLoginIsSaved()
        {
            string logintxt = "IGLogin.txt";
            string[] logininfo = { "", "" };
            int i = 0;
            if (System.IO.File.Exists(logintxt))
            {
                foreach (string line in File.ReadLines(logintxt, Encoding.UTF8))
                {
                    logininfo[i] = line;
                    i++;
                }
                if(logininfo[0] != "")
                {
                    IGLogin = logininfo[0];
                    IGPW = logininfo[1];
                    startWithOwnProfile.Content = "mit eigenem Profil angemeldet";
                    spLogout.Visibility = Visibility.Visible;
                }

            }
        }

        private void CheckIfJDownloaderPathIsSaved()
        {


            List<string> ljd = new List<string>();
            using (StreamReader sr = new StreamReader(JDSettingsPath))
            {

                foreach (string line in File.ReadLines(JDSettingsPath, Encoding.UTF8))
                {
                    ljd.Add(line);
                }
                if (ljd.First().ToString() != null || ljd.First().ToString() != "")
                {
                    igVideoLocationLabel.Content = "Ordner gefunden: " + ljd.First().ToString();
                    igVideoLocationPath.Content = "Ordner ändern?";
                    memorizeIGVideoLocationSavePath.Content = "Gemerkt!";
                }
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


        private void onCreateIGListClicked(object sender, RoutedEventArgs e)
        {
            if (speicherort == "" || speicherort == null)
            {
                MessageBox.Show("Kein Speicherort ausgewählt, bitte wähle den Episoden Ordner");
            }
            else
            {
                if (igtitle.Text == "")
                {
                    igtitle.Text = "Heute";
                }
                //       string targetPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\" + igtitle.Text;
                speicherortQuellenLikeability = Directory.GetCurrentDirectory();
                targetPath = speicherort + @"\" + igtitle.Text;
                Directory.CreateDirectory(targetPath);
                if (startWithBotProfile.IsChecked == true)
                {
                    botProfileIsActive = true;
                }

                DateTimeCheck();
            }
        }

        private void CreateIGListAgain()
        {
            if (speicherort == "" || speicherort == null)
            {
                MessageBox.Show("Kein Speicherort ausgewählt, bitte wähle den Episoden Ordner");
            }
            else
            {
                if (igtitle.Text == "")
                {
                    igtitle.Text = "Heute";
                }
                //       string targetPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\" + igtitle.Text;
                speicherortQuellenLikeability = Directory.GetCurrentDirectory();
                targetPath = speicherort + @"\" + igtitle.Text;
                Directory.CreateDirectory(targetPath);
                DateTimeCheck();
            }
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

        protected void WaitForPageLoad(IWebDriver driver)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            wait.Until(drivers => ((IJavaScriptExecutor)drivers).ExecuteScript("return document.readyState").Equals("complete"));
        }

        public void GetHTMLInfo(List<string> profileList, DateTime suggestedDate)
        {

            string error = "";
            int counter = 0;
            string creatorName = "";
            File.WriteAllText(speicherort + "\\Quellen.txt", String.Empty);
            File.WriteAllText(speicherortQuellenLikeability + "\\Quellen_Likeability.txt", String.Empty);


            ChromeOptions options = new ChromeOptions();
            options.AddArguments(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Local\Google\Chrome\User Data\Default");
            // options.AddArguments(@"C:\Users\Agrre\AppData\Local\Google\Chrome\User Data\Default");
            var service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            IWebDriver driver = new ChromeDriver(service, new ChromeOptions());

            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(10);



            //  COOKIE MANAGEMENT !?
            if (isBotProfileBlocked == true)
            {
                //  GetLoginInformation();
                driver.Url = "https://www.instagram.com/accounts/login/";
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);
                driver.Manage().Cookies.AddCookie(new Cookie("scheißIG", "fickdeinemutter"));

                var username = driver.FindElement(By.CssSelector("input[name='username']"));
                username.SendKeys("slamdankbot@gmail.com");
                var password = driver.FindElement(By.CssSelector("input[name='password']"));
                password.SendKeys("qwertzu123!");
                password.SendKeys(Keys.Enter);
                var allCookies = driver.Manage().Cookies.AllCookies;
                using (StreamWriter cookieWriter = new StreamWriter(speicherort + "\\Cookies.data", true, Encoding.UTF8))
                {
                    foreach (var cookie in allCookies)
                    {
                        //driver.Manage().Cookies.AddCookie(cookie);

                        cookieWriter.Write(cookie.Name + ";" + cookie.Value + ";" + cookie.Domain + ";" + cookie.Path + ";" + cookie.Expiry + ";" + cookie.Secure);

                    }
                    cookieWriter.Close();
                }
            }

            if (isOwnProfileIsActive == true)
            {
                driver.Url = "https://www.instagram.com/accounts/login/";
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);
                driver.Manage().Cookies.AddCookie(new Cookie("scheißIG", "fickdeinemutter"));

                var username = driver.FindElement(By.CssSelector("input[name='username']"));
                username.SendKeys(IGLogin);
                var password = driver.FindElement(By.CssSelector("input[name='password']"));
                password.SendKeys(IGPW);
                password.SendKeys(Keys.Enter);
                var allCookies = driver.Manage().Cookies.AllCookies;
                using (StreamWriter cookieWriter = new StreamWriter(speicherort + "\\Cookies.data", true, Encoding.UTF8))
                {
                    foreach (var cookie in allCookies)
                    {
                        //driver.Manage().Cookies.AddCookie(cookie);

                        cookieWriter.Write(cookie.Name + ";" + cookie.Value + ";" + cookie.Domain + ";" + cookie.Path + ";" + cookie.Expiry + ";" + cookie.Secure);

                    }
                    cookieWriter.Close();
                }
            }

            //  Cookie ck = new Cookie(coo)
            ///  aufrufeRoh = driver.FindElement(By.ClassName("vcOH2")).Text;
            //var username = driver.FindElement(By.ClassName("_2hvTZ pexuQ zyHYP"));
            //username.SendKeys("h.ljumic@gmx.at");
            //var password = driver.FindElement(By.ClassName("_2hvTZ pexuQ zyHYP"));
            //password.SendKeys("*********!");
            //password.SendKeys(Keys.Enter);

            //driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(20000);
            Thread.Sleep(10000);
            //IGPB.Show();
            //
            //ProgressBar IGPBar = new ProgressBar()
            //{
            //    Minimum = 0,
            //    Maximum = profileList.Count,
            //}; ;
            //IGPB.Content = IGPBar;
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);

            while (counter < profileList.Count)
            {



                try
                {
                    #region BackgroundWorker
                    //backgroundWorker1.DoWork += backgroundWorker1_DoWork;
                    //backgroundWorker1.ProgressChanged += worker_ProgressChanged;
                    //progressBarIG
                    //progressBarIG.Minimum = 1;
                    //progressBarIG.Maximum = profileList.Count;
                    //progressBarIG.Value = 1;
                    //  BackgroundWorker worker = new BackgroundWorker();
                    //  worker.WorkerReportsProgress = true;
                    //  worker.DoWork += worker_DoWork;
                    // https://www.wpf-tutorial.com/misc-controls/the-progressbar-control/

                    #endregion


                    driver.Url = profileList.ElementAt(counter);
                    var html = driver.PageSource;
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(html);
                    creatorName = GetProfileName(profileList.ElementAt(counter));
                    // zweite


                    // Check if Bot was not blocked by Instagram
                    isBotProfileBlocked = CheckIfInstagramBlockedBotProfile(htmlDoc);

                    if (isBotProfileBlocked == false)
                    {
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

                        if (targetPath == "\\Heute")
                        {
                            targetPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                        }

                        //Select all nodes (while timeoutspan and scroll-range)
                        #endregion

                        var node1 = htmlDoc.DocumentNode.SelectNodes("//div[@class='v1Nh3 kIKUG  _bz0w']//a[@href]");
                        if (node1 != null)
                        {

                            using (StreamWriter writer = new StreamWriter(targetPath + "\\Quellen.txt", true, Encoding.UTF8))
                            {
                                using (StreamWriter likeAbilityWriter = new StreamWriter(speicherortQuellenLikeability + "\\Quellen_Likeability.txt", true, Encoding.UTF8))

                                {
                                    string link = "";
                                    DateTime returnValueDate;
                                    double returnValue;
                                    string linkending = "";

                                    foreach (var item in node1)
                                    {
                                        isBotProfileBlocked = CheckIfInstagramBlockedBotProfile(htmlDoc);

                                        link = "https://www.instagram.com" + item.Attributes["href"].Value;
                                    //    returnValue = LikeabilityExpress(link, driver);
                                        returnValueDate = DateTimeExpress(link, driver);
                                        returnValue = LikeabilityExpress(link, driver);

                                        // DateTime dt1 = new DateTime(2019, 8, 9, 20, 0, 0);
                                        if (returnValueDate > suggestedDate && returnValue != 0)
                                        {
                                            writer.Write("https://www.instagram.com" + item.Attributes["href"].Value + "\r\n");
                                            linkending = item.Attributes["href"].Value;
                                            linkending = linkending.Substring(3).Trim();
                                            linkending = linkending.Remove(linkending.IndexOf("/"));
                                            // likeAbilityWriter.Write(@"C:\Users\Agrre\Desktop\testproject\" + creatorName + linkending + ".mp4" + " | " + returnValue + "\r\n");
                                            likeAbilityWriter.Write(targetPath + @"\" + creatorName + linkending + ".mp4" + " | " + returnValue + "\r\n");
                                        }
                                        else
                                        {
                                            break;
                                        }
                                        // writer.Write(profileList);
                                    }
                                    likeAbilityWriter.Close();
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
                    }
                    else
                    {
                        break;
                    }
                    //      progressBarIG.Value += counter;
                    counter++;
                }
                catch (Exception e)
                {
                    using (StreamWriter writer = new StreamWriter(speicherort + "\\QuellenError.txt", true, Encoding.UTF8))

                    {
                        writer.Write(e + error);
                    }
                    continue;

                }

            }
            driver.Quit();
            if (isBotProfileBlocked == true && botProfileIsActive == false)
            {
                // MessageBox.Show("Bot Profile wurde gesperrt, versuche ohne von neu...");
                MessageBoxResult result = MessageBox.Show("Bot Profile wurde gesperrt, versuch mit Botprofil?", "GondoAssist - Fehler", MessageBoxButton.YesNoCancel);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        botProfileIsActive = true;
                        CreateIGListAgain();
                        break;
                    case MessageBoxResult.No:
                        MessageBox.Show("Oh well, too bad!", "My App");
                        break;
                    case MessageBoxResult.Cancel:
                        break;
                }

            }
            CopyQuellenToDebugFolder(targetPath);
            MessageBox.Show("Videosuche beendet.");
        }

        private void GetLoginInformation()
        {
            throw new NotImplementedException();
        }

        private void CopyQuellenToDebugFolder(string targetPath)
        {
            File.Copy(System.IO.Path.Combine(targetPath, "Quellen.txt"), System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Quellen.txt"), true);


        }

        private bool CheckIfInstagramBlockedBotProfile(HtmlDocument htmlDoc)
        {
            var doesPreExists = htmlDoc.DocumentNode.Descendants("pre").Any();
            if (doesPreExists == true)
            {
                return true;
            }
            else
            {
                return false;
            }


        }

        private string GetProfileName(string v)
        {
            string creatorName = v.Substring(26).Trim();
            creatorName = creatorName.Substring(0, creatorName.Length - 1);
            return creatorName + " - ";
        }

        private double LikeabilityExpress(string Link, IWebDriver driver)
        {

            string link = Link;

            //driver.Url = link;


            string aufrufeRoh, likesRoh = "";
            if (IsElementPresent(By.ClassName("vcOH2"), driver))
            {
                aufrufeRoh = driver.FindElement(By.ClassName("vcOH2")).Text;
                driver.FindElement(By.ClassName("vcOH2")).Click();
                // driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(1); // besser nicht wegen timeout error

                //  creatorName = driver.FindElement(By.ClassName("sqdOP yWX7d     _8A5w5   ZIAjV ")).Text;


                if (IsElementPresent(By.ClassName("vJRqr"), driver))
                {
                    likesRoh = driver.FindElement(By.ClassName("vJRqr")).Text;
                    var aufruf = aufrufeRoh.Split(' ').First();
                    var likes = likesRoh.Substring(likesRoh.IndexOf(' ') + 1).Trim();
                    likes = likes.Split(' ').First();
                    bool likeBool = IsNumericFromTryParse(likes);
                    bool aufrufBool = IsNumericFromTryParse(aufruf);

                    if (likeBool == true && aufrufBool == true)
                    {
                        double summe = double.Parse(likes, NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture("en-GB")) / double.Parse(aufruf, NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture("en-GB"));


                        summe = summe * 100;

                        if (summe <= 100)
                        {
                            summe = Math.Round(summe, 2);
                            return summe;
                        }
                        if (summe >= 1000 && summe <= 10000)
                        {
                            summe = summe / 100;
                        }
                        else if (summe >= 10000)
                        {
                            summe = summe / 1000;
                        }
                        //    else if ()
                        summe = Math.Round(summe, 2);
                        return summe;


                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    //do if does not exists

                }
            }
            else
            {

                //do if does not exists

            }
            //  var aufrufeRoh = driver.FindElement(By.ClassName("vcOH2")).Text;



            return 0.00;
        }

        private static bool IsNumericFromTryParse(string likeaufrufe)
        {
            double result = 0;
            return (double.TryParse(likeaufrufe, NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture("en-GB"), out result));
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

        private DateTime DateTimeExpress(string Link, IWebDriver driver)
        {

            string link = Link;

            driver.Url = link;

            var html = driver.PageSource;
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            htmlDoc.OptionEmptyCollection = true;

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

        private void onIGSelectPathClicked(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog() { Description = "Select your Path" })
            {
                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    destinationPath = fbd.SelectedPath;
                }
            }

        }

        private void collectVideos(object sender, RoutedEventArgs e)
        {

            if (destinationPath == null || destinationPath == "")
            {
                MessageBox.Show("Kein SPEICHERORT für die Episode ausgewählt, bitte wähle den Episoden Ordner");
            }
            else
            {

                if (memorizeIGVideoLocationSavePath.IsChecked == true)
                {
                    File.WriteAllText(Directory.GetCurrentDirectory() + "\\JDownloaderLocation.txt", String.Empty);
                    using (StreamWriter writer = new StreamWriter(Directory.GetCurrentDirectory() + "\\JDownloaderLocation.txt", true, Encoding.UTF8))
                    {
                        writer.WriteLine(sourcePath);
                    }

                }
                using (StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + "\\JDownloaderLocation.txt"))
                {
                    sourcePath = sr.ReadLine();
                }



                //string sourcePath = @"C:\Users\E\Desktop\JdownloadSlamdan";
                //if (igtitle.Text == "")
                //{
                //    igtitle.Text = "Heute";
                //}
                string targetPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\" + igtitle.Text;
                //  string targetPath = speicherort + @"\" + igtitle.Text;
                string directoryName;
                string destDirectory;
                string fileName;
                string destFile;
                Directory.CreateDirectory(targetPath);
                // check if directory exists

                if (Directory.Exists(sourcePath))
                {
                    //  Directory.CreateDirectory(targetPath);
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
        }

        private void onLoginWithOwnProfileClicked(object sender, RoutedEventArgs e)
        {
            if (IGLogin != "" && IGPW != "")
            {

            }
            else
            {
                spIGLogin.Visibility = Visibility.Visible;
            }
        }

        private void onIGLoginClicked(object sender, RoutedEventArgs e)
        {
            // check for login merken?
            isOwnProfileIsActive = true;
            IGLogin = tbLogin.Text;
            IGPW = tbPW.Password;
            



            if (cbIGLogin.IsChecked == true)
            {
                //string hashedIGLogin = EncryptLoginInfo(IGLogin);
                //string hashedIGPW = EncryptLoginInfo(IGPW);

                using (StreamWriter sw = new StreamWriter(Directory.GetCurrentDirectory() + "\\IGLogin.txt", true, Encoding.UTF8))
                {
                    sw.WriteLine(IGLogin);
                    sw.WriteLine(IGPW);
                }
            }
            MessageBox.Show("Logindaten wurden gespeichert");
            spIGLogin.Visibility = Visibility.Collapsed;
            spLogout.Visibility = Visibility.Visible;
        }

        private void RetrieveIGLoginInfo()
        {

        }

        private void onLogoutClicked(object sender, RoutedEventArgs e)
        {
            IGLogin = "";
            IGPW = "";
            File.WriteAllText(Directory.GetCurrentDirectory() + "\\IGLogin.txt", String.Empty);
            spLogout.Visibility = Visibility.Collapsed;
            spIGLogin.Visibility = Visibility.Visible;
            tbLogin.Text = "";
            tbPW.Password = "";
        }

        private string EncryptLoginInfo(string value)
        {
            SHA256 sha256 = SHA256.Create();

            byte[] hashData = sha256.ComputeHash(Encoding.Default.GetBytes(value));
            StringBuilder returnValue = new StringBuilder();

            for (int i = 0; i < hashData.Length; i++)
            {
                returnValue.Append(hashData[i].ToString());
            }

            return returnValue.ToString();
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
