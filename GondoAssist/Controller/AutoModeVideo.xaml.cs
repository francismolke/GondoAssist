using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using GondoAssist.Klassen;

namespace GondoAssist.Controller
{
    /// <summary>
    /// Interaktionslogik für AutoModeVideo.xaml
    /// </summary>
    public partial class AutoModeVideo : UserControl
    {
        string speicherort, speicherordner = "";
        

        public AutoModeVideo()
        {
            InitializeComponent();
            createTags.IsEnabled = false;
        }

        private void onEpisodeSavePathClicked(object sender, RoutedEventArgs e)
        {
            try
            {

            using (System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog() { Description = "Select your Path" })
            {
                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    speicherort = fbd.SelectedPath;
                }
            }
            if (speicherort != "")
            {
            
            }

            }
            catch(Exception ex)
            {
                MessageBox.Show("da gab es wohl ein Fehler: " + ex.Message);

            }
            //finally
            //{
            //    MessageBox.Show("Fertig! Tags wurden hinzugefügt.");
            //    createTags.IsEnabled = false;
            //}
        }

        private void onCreateEpisodeClicked(object sender, RoutedEventArgs e)
        {
            AutoModeLikeability aml = new AutoModeLikeability();            
            aml.CreateBlankProjekt(episodeTitle.Text, speicherort);
        }

        private void onEpisodeOpenPathClicked(object sender, RoutedEventArgs e)
        {

        }

        private void InsertTagsIntoEpisode(object sender, RoutedEventArgs e)
        {
            bool notfinished = false;
            try
            { 
            if (speicherort == "")
            {
                MessageBox.Show("Kein Projekt angegeben");

            }
            if (speicherordner == "")
            {
                MessageBox.Show("Speicherort nicht angegeben");

            }
            if(speicherort != "" && speicherordner != "")
            {
            CreateTags createTags = new CreateTags();
            createTags.RunTags(speicherort, speicherordner);
            }
            }
            catch (Exception ex)
            {
                MessageBox.Show("da gab es wohl ein Fehler: " + ex.Message);

            }
            finally
            {
                MessageBox.Show("Fertig! Tags wurden hinzugefügt.");
                createTags.IsEnabled = false;
            }


        }

        private void OpenAndSaveProjectFile(object sender, RoutedEventArgs e)
        {
         
            //if(speicherort == "")
            //{
                using (System.Windows.Forms.OpenFileDialog fd = new System.Windows.Forms.OpenFileDialog())
                {
                    if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                      //  speicherordner = fd.dire;
                        speicherort = fd.FileName;

                    }
                }
            //}
            //else
            //{
            //  //  Process.Start("Slamdank1.wlmp");
            //  // Mach was
            //}

            speicherordner = System.IO.Path.GetDirectoryName(speicherort);

            speicherordner = speicherordner + @"\";
            ProcessStartInfo startInfo = new ProcessStartInfo();
           // startInfo.WorkingDirectory = speicherordner;
            startInfo.FileName = speicherort;
            if (startInfo.FileName == "")
            {
                MessageBox.Show("Kein Projekt angegeben");
            }
            else
            { 
            Process.Start(startInfo);
            }
            createTags.IsEnabled = true;
        }
    }
}
