using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using WordPressPCL;
using WordPressPCL.Models;

namespace GondoAssist
{
    /// <summary>
    /// Interaktionslogik für Uploader_Wordpress.xaml
    /// </summary>
    public partial class Uploader_Wordpress : UserControl
    {
        string xmlfilepath;
        string speicherort;
        string Titel, Beschreibung, Tags, Pfad;
        string privacystatus;
        string GDdatentyp;
        string _filePath;
        string TNSavePath, TNSaveFile;

        public Uploader_Wordpress()
        {
            InitializeComponent();
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
        private async void onPostClicked(object sender, RoutedEventArgs e)
        {
            await CreatePost(TNSavePath, TNSaveFile, wtitletb.Text, wDescriptionBox.Text);
        }

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

        private static async Task<WordPressClient> GetClient()
        {
            //JWT authentication
            var client = new WordPressClient("http://www.slamdank31.com/wp-json/");
            client.AuthMethod = AuthMethod.JWT;
            await client.RequestJWToken("Arschloch1", "Slimtwix13");
            return client;
        }


    }
}
