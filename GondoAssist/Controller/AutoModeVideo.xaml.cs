﻿using GondoAssist.Klassen;
using Google.Apis.Drive.v3.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

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
            GetDateForQuellen();
        }

        private void GetDateForQuellen()
        {
            try
            {

                string[] filename = { "Quellen.txt", "Quellen_Profile.txt", "Quellen_Likeability.txt" };
                for (int i = 0; i < filename.Length; i++)
                {
                    if (System.IO.File.Exists(filename[i]))
                    {
                        FileInfo fi = new FileInfo(filename[i]);
                      //  DateTime dt = fi.CreationTime;
                        DateTime dt = fi.LastWriteTime;

                        if (filename[i] == filename[0])
                        {
                            lbQuellen.Content += "(" + dt + ")";
                        }
                        if (filename[i] == filename[1])
                        {
                            lbQuellenProfile.Content += "(" + dt + ")";
                        }
                        if (filename[i] == filename[2])
                        {
                            lbQuellenLike.Content += "(" + dt + ")";
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
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
            catch (Exception ex)
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
            try
            {

                if (cbTwentymin.IsChecked == false && cbTenmin.IsChecked == false && cbAll.IsChecked == false && tbepslength.Text == "")
                {
                    MessageBox.Show("Bitte die Dauer des Episode auswählen: ");
                }
                if (tbepslength.Text != "" && cbTwentymin.IsChecked == false && cbTenmin.IsChecked == false && cbAll.IsChecked == false)
                {
                    int episodeLength = Int32.Parse(tbepslength.Text);
                    AutoModeLikeability aml = new AutoModeLikeability(cbTenmin.IsChecked, cbTwentymin.IsChecked, cbAll.IsChecked, 0, episodeLength);
                    aml.CreateBlankProjekt(episodeTitle.Text, speicherort);

                }
                else if (cbByLikeability.IsChecked == false && cbByCategoryThenLikeability.IsChecked == false)
                {
                    MessageBox.Show("Sortierreihenfolge wurde nicht gewählt");
                }
                else if (cbByLikeability.IsChecked == true && (cbTwentymin.IsChecked == true || cbTenmin.IsChecked == true || cbAll.IsChecked == true) && tbepslength.Text == "")
                {
                    // 0 = Likeablity
                    AutoModeLikeability aml = new AutoModeLikeability(cbTenmin.IsChecked, cbTwentymin.IsChecked, cbAll.IsChecked, 0, 0);
                    aml.CreateBlankProjekt(episodeTitle.Text, speicherort);
                }
                else if (cbByCategoryThenLikeability.IsChecked == true && (cbTwentymin.IsChecked == true || cbTenmin.IsChecked == true || cbAll.IsChecked == true) && tbepslength.Text == "")
                {
                    // 1 = Category & Likeability
                    AutoModeLikeability aml = new AutoModeLikeability(cbTenmin.IsChecked, cbTwentymin.IsChecked, cbAll.IsChecked, 1, 0);
                    aml.CreateBlankProjekt(episodeTitle.Text, speicherort);
                }
                //if (cbTwentymin.IsChecked == false && cbTenmin.IsChecked == false && cbAll.IsChecked == false)
                //{
                //    MessageBox.Show("Bitte die Dauer des Episode auswählen: ");
                //}

                //if (cbByLikeability.IsChecked == false && cbByCategoryThenLikeability.IsChecked == false)
                //{
                //    MessageBox.Show("Sortierreihenfolge wurde nicht gewählt");
                //}
                //if (cbByLikeability.IsChecked == true && (cbTwentymin.IsChecked == true || cbTenmin.IsChecked == true || cbAll.IsChecked == true))
                //{
                //    // 0 = Likeablity
                //    AutoModeLikeability aml = new AutoModeLikeability(cbTenmin.IsChecked, cbTwentymin.IsChecked, cbAll.IsChecked, 0);
                //    aml.CreateBlankProjekt(episodeTitle.Text, speicherort);
                //}
                //if (cbByCategoryThenLikeability.IsChecked == true && (cbTwentymin.IsChecked == true || cbTenmin.IsChecked == true || cbAll.IsChecked == true))
                //{
                //    // 1 = Category & Likeability
                //    AutoModeLikeability aml = new AutoModeLikeability(cbTenmin.IsChecked, cbTwentymin.IsChecked, cbAll.IsChecked, 1);
                //    aml.CreateBlankProjekt(episodeTitle.Text, speicherort);
                //}

            }
            catch(Exception ex)
            {
                using (StreamWriter sw = new StreamWriter("Error.txt", true, Encoding.UTF8))
                {
                    sw.WriteLine(ex.Message + ex.ToString());
                    // Get stack trace for the exception with source file information
                    var st = new StackTrace(ex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(0);
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();
                    sw.WriteLine(st + " " + frame + " " + line + " ");
                }
            }
        }

        private void onEpisodeOpenPathClicked(object sender, RoutedEventArgs e)
        {

        }

        List<string> blackList = new List<string>();
        private void InsertTagsIntoEpisode(object sender, RoutedEventArgs e)
        {
            string sourcepathBL = "BlackListforTags.txt";
            if (System.IO.File.Exists(sourcepathBL))
            {
                using (StreamReader filereader = new StreamReader(sourcepathBL))
                {
                    foreach (string line in System.IO.File.ReadLines(sourcepathBL, Encoding.UTF8))
                    {
                        blackList.Add(line);
                    }

                }
            }
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
                if (speicherort != "" && speicherordner != "")
                {
                    
                    CreateTags createTags = new CreateTags();
                    createTags.RunTags(speicherort, speicherordner, blackList);
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

        private void onQuellenOpenClicked(object sender, RoutedEventArgs e)
        {
            string filename = "Quellen.txt";
            FileInfo fi = new FileInfo(filename);
            DateTime dt = fi.CreationTime;
            if (System.IO.File.Exists(filename))
            {
                Process.Start(filename);
            }
            else
            {
                MessageBox.Show("Es ist keine Quellen Datei verfügbar");
            }
        }

        private void onQuellenProfileOpenClicked(object sender, RoutedEventArgs e)
        {
            string filename = "Quellen_Profile.txt";
            if (System.IO.File.Exists(filename))
            {
                Process.Start(filename);
            }
            else
            {
                MessageBox.Show("Es ist keine Quellen Datei verfügbar");
            }

        }

        private void onQuellenLikeOpenClicked(object sender, RoutedEventArgs e)
        {
            string filename = "Quellen_Likeability.txt";
            if (System.IO.File.Exists(filename))
            {
                Process.Start(filename);
            }
            else
            {
                MessageBox.Show("Es ist keine Quellen Datei verfügbar");
            }
        }
        private void EpisodeLengthTextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbepslength.Text == "")
            {
                cbTenmin.IsEnabled = true;
                cbTwentymin.IsEnabled = true;
                cbAll.IsEnabled = true;
            }
            if (tbepslength.Text != "")
            {
                cbTenmin.IsEnabled = false;
                cbTwentymin.IsEnabled = false;
                cbAll.IsEnabled = false;
            }
        }
        private void onTenMinsClicked(object sender, RoutedEventArgs e)
        {
            //// Tenmin Checkbox enabled
            //if(cbTenmin.IsChecked == true)
            //{
            //    cbTwentymin.IsEnabled = false;
            //    cbAll.IsEnabled = false;
            //}

            //if (cbTenmin.IsChecked == false)
            //{
            //    cbTwentymin.IsEnabled = true;
            //    cbAll.IsEnabled = true;
            //}

            ////TwentyMin Checkbox enabled
            //if (cbTwentymin.IsChecked == true)
            //{
            //    cbTenmin.IsEnabled = false;
            //    cbAll.IsEnabled = false;
            //}

            //if (cbTwentymin.IsChecked == false)
            //{
            //    cbTenmin.IsEnabled = true;
            //    cbAll.IsEnabled = true;
            //}

            ////All Checkbox enabled

            //if (cbTwentymin.IsChecked == true)
            //{
            //    cbTenmin.IsEnabled = false;
            //    cbAll.IsEnabled = false;
            //}

            //if (cbTwentymin.IsChecked == false)
            //{
            //    cbTenmin.IsEnabled = true;
            //    cbAll.IsEnabled = true;
            //}
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
