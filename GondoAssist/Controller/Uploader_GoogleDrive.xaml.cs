using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace GondoAssist
{
    /// <summary>
    /// Interaktionslogik für Uploader_GoogleDrive.xaml
    /// </summary>
    public partial class Uploader_GoogleDrive : UserControl
    {
        string _filePath;
        string GDdatentyp;
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



        public Uploader_GoogleDrive()
        {
            InitializeComponent();
        }

        private void onGDSelectFile(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _filePath = ofd.FileName;
                }
        }

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

        private static DriveService GetDriveService(UserCredential credential)
        {
            return new DriveService(
                new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName
                });
        }

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

    }
}
