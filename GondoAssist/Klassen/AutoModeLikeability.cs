using MediaToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace GondoAssist.Klassen
{
    public class AutoModeLikeability
    {
        bool? cbTen, cbTwenty, cbAll;
        int sortSequence;

        public AutoModeLikeability(bool? _cbTen, bool? _cbTwenty, bool? _cbAll, int _sortSequence)
        {
            //CheckBox _cbTen, CheckBox _cbTwenty, CheckBox _cbAll
            this.cbTen = _cbTen;
            this.cbTwenty = _cbTwenty;
            this.cbAll = _cbAll;
            this.sortSequence = _sortSequence;
        }

        public void CreateBlankProjekt(string projektName, string speicherOrt)
        {
            List<string> lfilepath = new List<string>();
            string[] filepath = null;
            try
            {
                string projektDatei = projektName + ".wlmp";
               
                if (sortSequence == 0)
                {
                filepath = GetLikeability();
                }
                if (sortSequence == 1)
                {
                filepath = GetLikeabilityByCategory();                  
                }


                
                Tuple<int, int> getValue = new Tuple<int, int>(GetFramesizeOfVideos(filepath, 0).Item1, GetFramesizeOfVideos(filepath, 0).Item2);
                //int width = 1920;
                //int height = 1080;
                int width = getValue.Item1;
                int height = getValue.Item2;
                string newDirectiory = speicherOrt + @"\" + projektDatei;
                double duration = GetDurationOfVideos(filepath, 0);
                using (XmlWriter writer = XmlWriter.Create(newDirectiory))
                {
                    writer.WriteStartDocument();
                }


                // CREATES PROJECT/MEDIAITEMS/MEDIAITEM on EMPTY XML
                new XDocument(
                        new XElement("Project", new XAttribute("name", projektName), new XAttribute("themeId", 0), new XAttribute("version", 65540), new XAttribute("templateID", "SimpleProjectTemplate"),
                                    new XElement(new XElement("MediaItems", new XElement("MediaItem", new XAttribute("id", 1), new XAttribute("filePath", filepath[0]), new XAttribute("arWidth", width), new XAttribute("arHeight", height), new XAttribute("duration", duration), new XAttribute("songTitle", ""), new XAttribute("songArtist", ""), new XAttribute("songAlbum", ""), new XAttribute("songCopyrightUrl", ""), new XAttribute("songArtistUrl", ""), new XAttribute("songAudioFileUrl", ""), new XAttribute("stabilizationMode", 0), new XAttribute("mediaItemType", 1)))),
                                    new XElement("Extents", ""))).Save(newDirectiory);
                XDocument doc = XDocument.Load(newDirectiory);

                CreateProject(newDirectiory, filepath.Length, doc, filepath, projektDatei);
            }
            catch (Exception e)
            {
                using (StreamWriter sr = new StreamWriter(Directory.GetCurrentDirectory() + "\\ErrorProject.txt", true, Encoding.UTF8))
                {
                    MessageBox.Show("Da gab es ein Problem:" + e.Message);
                    sr.WriteLine(e.Message.ToString() + e.Message);
                }
            }
        }



        private static Tuple<int, int> GetFramesizeOfVideos(string[] allvideos, int position)
        {

            string swidth, sheight;
            int width = 640, height = 640;
            try
            {

                var inputFile = new MediaToolkit.Model.MediaFile { Filename = allvideos[position] };

                using (var engine = new Engine())
                {
                    engine.GetMetadata(inputFile);
                }
                var framesize = inputFile.Metadata.VideoData.FrameSize;
                swidth = framesize.Substring(0, framesize.IndexOf("x")).TrimEnd();
                sheight = framesize.Substring(framesize.IndexOf("x") + 1).Trim();

                width = Int32.Parse(swidth);
                height = Int32.Parse(sheight);

            }
            catch (FileNotFoundException fe)
            {
                using (StreamWriter sr = new StreamWriter(Directory.GetCurrentDirectory() + "\\MediaToolKitError.txt", true, Encoding.UTF8))
                {
                    sr.WriteLine(fe.Message.ToString() + fe.Message);
                }
            }
            return Tuple.Create(width, height);

        }

        private static double GetDurationOfVideos(string[] filepath, int n)
        {
            var inputFile = new MediaToolkit.Model.MediaFile { Filename = filepath[n] };
            double duration;
            int minute = 0;
            using (var engine = new Engine())
            {
                engine.GetMetadata(inputFile);
            }

            //Console.WriteLine(inputFile.Metadata.Duration);
            var mediaDuration = inputFile.Metadata.Duration;
            // Console.WriteLine(inputFile.Metadata.VideoData.FrameSize);

            string mediaDurationstring = mediaDuration.ToString();
            if (mediaDurationstring.Substring(0, mediaDurationstring.IndexOf(":")).TrimEnd() == "00")
            {
                mediaDurationstring = mediaDurationstring.Substring(3).Trim();

                if (mediaDurationstring.Substring(0, mediaDurationstring.IndexOf(":")).TrimEnd() == "00")
                {
                    mediaDurationstring = mediaDurationstring.Substring(3).Trim();
                }
                else
                {
                    var zwminute = mediaDurationstring.Substring(0, mediaDurationstring.IndexOf(":")).TrimEnd();
                    minute = Int32.Parse(zwminute);
                    mediaDurationstring = mediaDurationstring.Substring(3).TrimEnd();
                }
                if (mediaDurationstring.Substring(0).TrimEnd() != "00")
                {

                    duration = double.Parse(mediaDurationstring, NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture("en-GB"));
                    if (minute != 0)
                    {
                        minute = minute * 60;
                        duration = Math.Round(duration + minute, 2);
                        return duration;
                    }
                }

            }
            else
            {
                return double.Parse(mediaDurationstring);
            }
            // 00:00:08.1700000
            //  CreateBlankProjekt();
            n++;
            return double.Parse(mediaDurationstring, NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture("en-GB"));

            //}
            //return 0.00;



        }

        private static double GetDurationOfVideos(string filepath)
        {
            // string[] allvideos = GetAllVideosFromFolder();
            //int n = 0;
            //while (n < allvideos.Length)
            //{
            //GondoAssist_Tags.Program gat = new GondoAssist_Tags.Program();
            //   var inputFile = new MediaFile { Filename = @"C:\Users\Agrre\Desktop\testproject\_waifi_ - B8u7kJ7Fn8f.mp4" };
            var inputFile = new MediaToolkit.Model.MediaFile { Filename = filepath };
            double duration;
            int minute = 0;
            using (var engine = new Engine())
            {
                engine.GetMetadata(inputFile);
            }

            //Console.WriteLine(inputFile.Metadata.Duration);
            var mediaDuration = inputFile.Metadata.Duration;
            // Console.WriteLine(inputFile.Metadata.VideoData.FrameSize);

            string mediaDurationstring = mediaDuration.ToString();
            if (mediaDurationstring.Substring(0, mediaDurationstring.IndexOf(":")).TrimEnd() == "00")
            {
                mediaDurationstring = mediaDurationstring.Substring(3).Trim();

                if (mediaDurationstring.Substring(0, mediaDurationstring.IndexOf(":")).TrimEnd() == "00")
                {
                    mediaDurationstring = mediaDurationstring.Substring(3).Trim();
                }
                else
                {
                    var zwminute = mediaDurationstring.Substring(0, mediaDurationstring.IndexOf(":")).TrimEnd();
                    minute = Int32.Parse(zwminute);
                    mediaDurationstring = mediaDurationstring.Substring(3).TrimEnd();
                }
                if (mediaDurationstring.Substring(0).TrimEnd() != "00")
                {

                    duration = double.Parse(mediaDurationstring, NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture("en-GB"));
                    if (minute != 0)
                    {
                        minute = minute * 60;
                        duration = Math.Round(duration + minute, 2);
                        return duration;
                    }
                }

            }
            else
            {
                return double.Parse(mediaDurationstring);
            }
            // 00:00:08.1700000
            //  CreateBlankProjekt();

            return double.Parse(mediaDurationstring, NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture("en-GB"));

            //}
            //return 0.00;



        }

        private string[] GetLikeability()
        {
            string[] filePath = null;
            SortedList<int, LikeabilityQuellen> slClips = new SortedList<int, LikeabilityQuellen>();
            List<double> ld = new List<double>();
            List<LikeabilityQuellen> lg = new List<LikeabilityQuellen>();
            string line;
            string likestringValue;
            double likeValue;
            double tenMinDuration = 750;
            double twentyMinDuration = 1200;
            double duration = 0;
            string profilename = "";
            //  File.WriteAllText(speicherort + "\\Quellen.txt", String.Empty);
            List<LikeabilityQuellen> lgg = new List<LikeabilityQuellen>();

            //  using (StreamReader sr = new StreamReader("Quellen_Likeability.txt", Encoding.UTF8))
            using (StreamReader sr = new StreamReader("Quellen_Likeability.txt", Encoding.UTF8))
            {

                while ((line = sr.ReadLine()) != null)
                {

                    //    likestringValue = line.Substring(line.IndexOf("/ - ") + 3).Trim();
                    likestringValue = line.Substring(line.IndexOf(".mp4 | ") + 7).Trim();
                    likeValue = double.Parse(likestringValue, NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture("de-DE"));
                    //      likeValue = double.Parse(likestringValue, NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture("en-GB"));
                    //  line = line.Remove(line.IndexOf("/ - "));

                    // Gets the ProfileName
                    profilename = line.Substring(line.LastIndexOf("\\") + 1).Trim();
                    profilename = profilename.Substring(0, profilename.IndexOf(" - ")).Trim();


                    // Könnte Probleme machen?
                    line = line.Remove(line.IndexOf(".mp4 | ") + 4);
                    //line = line.Substring(3);


                    // uniquelist.ForEach()
                    if (File.Exists(line))
                    {
                        duration = GetDurationOfVideos(line);

                        lg.Add(new LikeabilityQuellen(likeValue, line, duration, profilename));

                    }
                    else
                    {
                        // mach nichts
                    }


                }

                List<LikeabilityQuellen> uniquelist = lg.GroupBy(i => i.Link).Select(g => g.First()).ToList();

                lgg = uniquelist.OrderBy(o => o.Likeability).ToList();
                lgg.Reverse();

                int n = 0;
                duration = 0;
                profilename = string.Empty;
                // Zehn Minuten Episode
                if (cbTen == true)
                {
                    while (duration < tenMinDuration)
                    {

                        // 5 < 12

                        duration += lgg.Select(d => d.Duration).Skip(n).First();
                        profilename = lgg.Select(p => p.ProfileName).Skip(n).First();


                        n++;

                    }
                    lgg.RemoveRange(n, lgg.Count - n);
                    foreach (var file in lgg)
                    {
                        filePath = lgg.Select(f => f.Link).ToArray();
                    }
                    MakeProfileListQuellen(lgg, n);

                    return filePath;
                }

                // Zwanzig Minuten episode
                if (cbTwenty == true)
                {

                    while (duration < twentyMinDuration)
                    {

                        // 5 < 12
                        var haha = lgg.Count;
                        if (n < lgg.Count)
                        {
                            duration += lgg.Select(d => d.Duration).Skip(n).First();
                            n++;
                        }
                        else
                        {
                            var diff = twentyMinDuration - duration;
                            duration = duration + diff + 1;
                        }

                    }
                    lgg.RemoveRange(n, lgg.Count - n);
                    foreach (var file in lgg)
                    {
                        filePath = lgg.Select(f => f.Link).ToArray();
                    }
                    MakeProfileListQuellen(lgg, n);
                    return filePath;
                    // zwanzig minuten episode
                }

                // Alle Clips verwenden
                if (cbAll == true)
                {
                    n = lgg.Count;
                    foreach (var file in lgg)
                    {
                        filePath = lgg.Select(f => f.Link).ToArray();
                    }
                    MakeProfileListQuellen(lgg, n);
                    return filePath;
                }


            }
            return filePath;
        }

        //private static List<LikeabilityQuellen> FillCategoryListWithProfiles(List<LikeabilityQuellen> list_Category)
        //{
        //    string line;
        //    string profilename;

        //    using (StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + "\\ProfilListen\\Top.txt", Encoding.UTF8))
        //    {
        //        while ((line = sr.ReadLine()) != null)
        //        {
        //            //   profilename = line.Substring(26).Trim();
        //            profilename = line.Substring(26);
        //            profilename = profilename.Substring(0, profilename.Length - 1);
        //            list_Category.Add(new LikeabilityQuellen(0, null, 0, profilename));
        //        }
        //    }
        //    using (StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + "\\ProfilListen\\Better.txt", Encoding.UTF8))
        //    {
        //        while ((line = sr.ReadLine()) != null)
        //        {
        //            //   profilename = line.Substring(26).Trim();
        //            profilename = line.Substring(26);
        //            profilename = profilename.Substring(0, profilename.Length - 1);
        //            //    list_Category.Add(profilename);
        //            list_Category.Add(new LikeabilityQuellen(0, null, 0, profilename));

        //        }
        //    }
        //    using (StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + "\\ProfilListen\\Good.txt", Encoding.UTF8))
        //    {
        //        while ((line = sr.ReadLine()) != null)
        //        {
        //            //   profilename = line.Substring(26).Trim();
        //            profilename = line.Substring(26);
        //            profilename = profilename.Substring(0, profilename.Length - 1);
        //            //list_Category.Add(profilename);
        //            list_Category.Add(new LikeabilityQuellen(0, null, 0, profilename));

        //        }
        //    }
        //    using (StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + "\\ProfilListen\\Middle.txt", Encoding.UTF8))
        //    {
        //        while ((line = sr.ReadLine()) != null)
        //        {
        //            //   profilename = line.Substring(26).Trim();
        //            profilename = line.Substring(26);
        //            profilename = profilename.Substring(0, profilename.Length - 1);
        //            //list_Category.Add(profilename);
        //            list_Category.Add(new LikeabilityQuellen(0, null, 0, profilename));

        //        }
        //    }
        //    using (StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + "\\ProfilListen\\Filler.txt", Encoding.UTF8))
        //    {
        //        while ((line = sr.ReadLine()) != null)
        //        {
        //            //   profilename = line.Substring(26).Trim();
        //            profilename = line.Substring(26);
        //            profilename = profilename.Substring(0, profilename.Length - 1);
        //            //list_Category.Add(profilename);
        //            list_Category.Add(new LikeabilityQuellen(0, null, 0, profilename));

        //        }
        //    }
        //    using (StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + "\\ProfilListen\\End.txt", Encoding.UTF8))
        //    {
        //        while ((line = sr.ReadLine()) != null)
        //        {
        //            //   profilename = line.Substring(26).Trim();
        //            profilename = line.Substring(26);
        //            profilename = profilename.Substring(0, profilename.Length - 1);
        //            //list_Category.Add(profilename);
        //            list_Category.Add(new LikeabilityQuellen(0, null, 0, profilename));

        //        }
        //    }
        //    return list_Category;
        //}

        private static List<LikeabilityQuellen> FillCategoryListWithProfiles(int i, List<LikeabilityQuellen> lTop, List<LikeabilityQuellen> lBetter, List<LikeabilityQuellen> lGood, List<LikeabilityQuellen> lMiddle, List<LikeabilityQuellen> lFiller, List<LikeabilityQuellen> lEnd)
        {
            string line;
            string profilename;
            switch (i)
            {
                case 1:
                    using (StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + "\\ProfilListen\\Top.txt", Encoding.UTF8))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            //   profilename = line.Substring(26).Trim();
                            profilename = line.Substring(26);
                            profilename = profilename.Substring(0, profilename.Length - 1);
                            lTop.Add(new LikeabilityQuellen(0, "", 0, profilename));
                        }
                    }
                    return lTop;

                case 2:
                    using (StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + "\\ProfilListen\\Better.txt", Encoding.UTF8))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            //   profilename = line.Substring(26).Trim();
                            profilename = line.Substring(26);
                            profilename = profilename.Substring(0, profilename.Length - 1);
                            lBetter.Add(new LikeabilityQuellen(0, "", 0, profilename));
                        }
                    }
                    return lBetter;
                case 3:
                    using (StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + "\\ProfilListen\\Good.txt", Encoding.UTF8))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            //   profilename = line.Substring(26).Trim();
                            profilename = line.Substring(26);
                            profilename = profilename.Substring(0, profilename.Length - 1);
                            lGood.Add(new LikeabilityQuellen(0, "", 0, profilename));
                        }
                    }
                    return lGood;
                case 4:
                    using (StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + "\\ProfilListen\\Middle.txt", Encoding.UTF8))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            //   profilename = line.Substring(26).Trim();
                            profilename = line.Substring(26);
                            profilename = profilename.Substring(0, profilename.Length - 1);
                            lMiddle.Add(new LikeabilityQuellen(0, "", 0, profilename));
                        }
                    }
                    return lMiddle;
                case 5:
                    using (StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + "\\ProfilListen\\Filler.txt", Encoding.UTF8))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            //   profilename = line.Substring(26).Trim();
                            profilename = line.Substring(26);
                            profilename = profilename.Substring(0, profilename.Length - 1);
                            lFiller.Add(new LikeabilityQuellen(0, "", 0, profilename));
                        }
                    }
                    return lFiller;
                case 6:
                    using (StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + "\\ProfilListen\\End.txt", Encoding.UTF8))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            //   profilename = line.Substring(26).Trim();
                            profilename = line.Substring(26);
                            profilename = profilename.Substring(0, profilename.Length - 1);
                            lEnd.Add(new LikeabilityQuellen(0, "", 0, profilename));
                        }
                    }
                    return lEnd;
            }
            // nicht null returnen vl
            return null;
        }

        List<LikeabilityQuellen> lTop = new List<LikeabilityQuellen>();
       // List<string> lTop = new List<string>();
        List<LikeabilityQuellen> lBetter = new List<LikeabilityQuellen>();
        List<LikeabilityQuellen> lGood = new List<LikeabilityQuellen>();
        List<LikeabilityQuellen> lMiddle = new List<LikeabilityQuellen>();
        List<LikeabilityQuellen> lFiller = new List<LikeabilityQuellen>();
        List<LikeabilityQuellen> lEnd = new List<LikeabilityQuellen>();

        private string[] GetLikeabilityByCategory()
        {
            // List<string> list_Category = new List<string>();
            List<LikeabilityQuellen> list_Category = new List<LikeabilityQuellen>();
            string[] filePath = null;
            SortedList<int, LikeabilityQuellen> slClips = new SortedList<int, LikeabilityQuellen>();
            List<double> ld = new List<double>();
            List<LikeabilityQuellen> lg = new List<LikeabilityQuellen>();
            string line;
            string likestringValue;
            double likeValue;
            double tenMinDuration = 750;
            double twentyMinDuration = 1200;
            double duration = 0;
            string profilename = "";
            // int n = 0;
            List<LikeabilityQuellen> lgg = new List<LikeabilityQuellen>();

            // Category Lists
            // Top, Better, Good, Middle, Filler, End
            for (int i = 1; i <= 6; i++)
            {
                // FillCategoryListWithProfiles(list_Category);
                //FillCategoryListWithProfilez(list_Category);
                FillCategoryListWithProfiles(i, lTop, lBetter, lGood, lMiddle, lFiller, lEnd);

            }




            using (StreamReader sr = new StreamReader("Quellen_Likeability.txt", Encoding.UTF8))
            {

                while ((line = sr.ReadLine()) != null)
                {

                    likestringValue = line.Substring(line.IndexOf(".mp4 | ") + 7).Trim();
                    likeValue = double.Parse(likestringValue, NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture("de-DE"));


                    // Gets the ProfileName
                    profilename = line.Substring(line.LastIndexOf("\\") + 1).Trim();
                    profilename = profilename.Substring(0, profilename.IndexOf(" - ")).Trim();


                    // Könnte Probleme machen?
                    line = line.Remove(line.IndexOf(".mp4 | ") + 4);
                    //line = line.Substring(3);



                    if (File.Exists(line))
                    {
                        duration = GetDurationOfVideos(line);

                        lg.Add(new LikeabilityQuellen(likeValue, line, duration, profilename));

                    }
                    else
                    {
                        // mach nichts
                    }


                }

                List<LikeabilityQuellen> uniquelist = lg.GroupBy(i => i.Link).Select(g => g.First()).ToList();
                //uniquelist.OrderBy(i => i.Likeability);

                var newListOfCategories = FillListByCategories(list_Category, uniquelist);




                int n = 0;
                duration = 0;
                profilename = string.Empty;

                // Zehn Minuten Episode
                if (cbTen == true)
                {
                    while (duration < tenMinDuration)
                    {

                        // 5 < 12

                        duration += newListOfCategories.Select(d => d.Duration).Skip(n).First();
                        profilename = newListOfCategories.Select(p => p.ProfileName).Skip(n).First();


                        n++;

                    }
                    newListOfCategories.RemoveRange(n, newListOfCategories.Count - n);
                    foreach (var file in newListOfCategories)
                    {
                        filePath = newListOfCategories.Select(f => f.Link).ToArray();
                    }
                    MakeProfileListQuellen(newListOfCategories, n);

                    return filePath;
                }

                // Zwanzig Minuten episode
                if (cbTwenty == true)
                {

                    while (duration < twentyMinDuration)
                    {

                        // 5 < 12
                        var haha = newListOfCategories.Count;
                        if (n < newListOfCategories.Count)
                        {
                            duration += newListOfCategories.Select(d => d.Duration).Skip(n).First();
                            n++;
                        }
                        else
                        {
                            var diff = twentyMinDuration - duration;
                            duration = duration + diff + 1;
                        }

                    }
                    newListOfCategories.RemoveRange(n, newListOfCategories.Count - n);
                    foreach (var file in newListOfCategories)
                    {
                        filePath = newListOfCategories.Select(f => f.Link).ToArray();
                    }
                    MakeProfileListQuellen(newListOfCategories, n);
                    return filePath;
                    // zwanzig minuten episode
                }

                // Alle Clips verwenden
                if (cbAll == true)
                {
                    n = newListOfCategories.Count;
                    foreach (var file in newListOfCategories)
                    {
                        filePath = newListOfCategories.Select(f => f.Link).ToArray();
                    }
                    MakeProfileListQuellen(newListOfCategories, n);
                    return filePath;
                }



            }

            return null;

        }

        private List<LikeabilityQuellen> FillListByCategories(List<LikeabilityQuellen> list_Category, List<LikeabilityQuellen> uniquelist)
        {
            List<LikeabilityQuellen> likeList;

            List<LikeabilityQuellen> fillerList = new List<LikeabilityQuellen>();
            // Erster die Top auffüllen
            for (int i = 0; i < lTop.Count; i++)
            {
                for (int n = 0; n < uniquelist.Count; n++)
                {
                    if (lTop.Skip(i).First().ProfileName == uniquelist.Skip(n).First().ProfileName)
                    {
                        // list_Category.Add(uniquelist.Skip(n).First());
                        fillerList.Add(uniquelist.Skip(n).First());
                    }
                    else
                    {
                        // nix
                    }
                }

            }
            likeList = fillerList.OrderBy(l => l.Likeability).ToList();
            likeList.Reverse();
            list_Category.AddRange(likeList);
            fillerList.Clear();
            likeList.Clear();
            // Better auffüllen
            for (int i = 0; i < lBetter.Count; i++)
            {
                for (int n = 0; n < uniquelist.Count; n++)
                {
                    if (lBetter.Skip(i).First().ProfileName == uniquelist.Skip(n).First().ProfileName)
                    {
                        fillerList.Add(uniquelist.Skip(n).First());
                    }
                    else
                    {
                        // nix
                    }
                }

            }
            fillerList.OrderBy(l => l.Likeability);
            list_Category.AddRange(fillerList);
            fillerList.Clear();
            // Good auffüllen
            for (int i = 0; i < lGood.Count; i++)
            {
                for (int n = 0; n < uniquelist.Count; n++)
                {
                    if (lGood.Skip(i).First().ProfileName == uniquelist.Skip(n).First().ProfileName)
                    {
                        fillerList.Add(uniquelist.Skip(n).First());
                    }
                    else
                    {
                        // nix
                    }
                }

            }
            fillerList.OrderBy(l => l.Likeability);
            list_Category.AddRange(fillerList);
            fillerList.Clear();
            // Middle auffüllen
            for (int i = 0; i < lMiddle.Count; i++)
            {
                for (int n = 0; n < uniquelist.Count; n++)
                {
                    if (lMiddle.Skip(i).First().ProfileName == uniquelist.Skip(n).First().ProfileName)
                    {
                        fillerList.Add(uniquelist.Skip(n).First());
                    }
                    else
                    {
                        // nix
                    }
                }

            }
            fillerList.OrderBy(l => l.Likeability);
            list_Category.AddRange(fillerList);
            fillerList.Clear();
            // Filler auffüllen
            for (int i = 0; i < lFiller.Count; i++)
            {
                for (int n = 0; n < uniquelist.Count; n++)
                {
                    if (lFiller.Skip(i).First().ProfileName == uniquelist.Skip(n).First().ProfileName)
                    {
                        fillerList.Add(uniquelist.Skip(n).First());
                    }
                    else
                    {
                        // nix
                    }
                }

            }
            fillerList.OrderBy(l => l.Likeability);
            list_Category.AddRange(fillerList);
            fillerList.Clear();
            // Ende auffüllen
            for (int i = 0; i < lEnd.Count; i++)
            {
                for (int n = 0; n < uniquelist.Count; n++)
                {
                    if (lEnd.Skip(i).First().ProfileName == uniquelist.Skip(n).First().ProfileName)
                    {
                        fillerList.Add(uniquelist.Skip(n).First());
                    }
                    else
                    {
                        // nix
                    }
                }

            }
            fillerList.OrderBy(l => l.Likeability);
            list_Category.AddRange(fillerList);
            fillerList.Clear();
            return list_Category;
        }

        private void FillCategoryListWithProfilez(List<LikeabilityQuellen> list_Category)
        {
            string line;
            string profilename;

            using (StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + "\\ProfilListen\\Top.txt", Encoding.UTF8))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    //   profilename = line.Substring(26).Trim();
                    profilename = line.Substring(26);
                    profilename = profilename.Substring(0, profilename.Length - 1);
                    // list_Category.Add(new LikeabilityQuellen(0, null, 0, profilename));
                }
            }
            using (StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + "\\ProfilListen\\Better.txt", Encoding.UTF8))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    //   profilename = line.Substring(26).Trim();
                    profilename = line.Substring(26);
                    profilename = profilename.Substring(0, profilename.Length - 1);
                    //    list_Category.Add(profilename);
                    //  list_Category.Add(new LikeabilityQuellen(0, null, 0, profilename));

                }
            }
            using (StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + "\\ProfilListen\\Good.txt", Encoding.UTF8))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    //   profilename = line.Substring(26).Trim();
                    profilename = line.Substring(26);
                    profilename = profilename.Substring(0, profilename.Length - 1);
                    //list_Category.Add(profilename);
                    //   list_Category.Add(new LikeabilityQuellen(0, null, 0, profilename));

                }
            }
            using (StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + "\\ProfilListen\\Middle.txt", Encoding.UTF8))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    //   profilename = line.Substring(26).Trim();
                    profilename = line.Substring(26);
                    profilename = profilename.Substring(0, profilename.Length - 1);
                    //list_Category.Add(profilename);
                    //  list_Category.Add(new LikeabilityQuellen(0, null, 0, profilename));

                }
            }
            using (StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + "\\ProfilListen\\Filler.txt", Encoding.UTF8))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    //   profilename = line.Substring(26).Trim();
                    profilename = line.Substring(26);
                    profilename = profilename.Substring(0, profilename.Length - 1);
                    //list_Category.Add(profilename);
                    //  list_Category.Add(new LikeabilityQuellen(0, null, 0, profilename));

                }
            }
            using (StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + "\\ProfilListen\\End.txt", Encoding.UTF8))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    //   profilename = line.Substring(26).Trim();
                    profilename = line.Substring(26);
                    profilename = profilename.Substring(0, profilename.Length - 1);
                    //list_Category.Add(profilename);
                    // list_Category.Add(new LikeabilityQuellen(0, null, 0, profilename));

                }
            }
        }

        private void MakeProfileListQuellen(List<LikeabilityQuellen> lgg, int n)
        {
            File.WriteAllText(Directory.GetCurrentDirectory() + "\\Quellen_Profile.txt", String.Empty);
            SortedSet<string> hsl = new SortedSet<string>();

            string profilename = "";

            for (int i = 0; i < n; i++)
            {
                profilename = lgg.Select(p => p.ProfileName).Skip(i).First();
                hsl.Add(profilename);
            }

            foreach (var pn in hsl)
            {
                // Wie timestamps
                using (StreamWriter sw = new StreamWriter(Directory.GetCurrentDirectory() + "\\Quellen_Profile.txt", true, Encoding.UTF8))
                {
                    sw.WriteLine("https://www.instagram.com/" + pn + @"/");
                }
            }

        }

        private static void CreateProject(string newDirectiory, int amountOfMediaItems, XDocument doc, string[] filepath, string projektDatei)
        {
            int mediaItemID = 1;

            // Creates Project / MediaItems 
            int letzteZahl = MakeOneMediaItemsWithOneNode(newDirectiory, mediaItemID, amountOfMediaItems, doc, filepath);
            letzteZahl += 5;

            // Insert ExtentSelector
            MakeExtentSelectorForAutomation(newDirectiory);

            // Insert BoundPlaceholders
            MakeBoundPlaceholders(newDirectiory);

            // Insert BoundProperties
            MakeBoundProperties(newDirectiory);

            // Insert ThemeOperationLog
            MakeThemeOperationLog(newDirectiory);

            // Insert AudioDuckingProperties
            MakeAudioDuckingProperties(newDirectiory);

            // Insert ExtentRefs id=1
            int extent4ID = MakeExtentRefForExtentRefs1(newDirectiory, letzteZahl, amountOfMediaItems);

            //  string currentDirectory = Directory.GetCurrentDirectory();
            // Startet das Tag Program
            // Tags.Program start = new Tags.Program();

            // start.RunTags(projektDatei, currentDirectory);
        }



        private static int MakeOneMediaItemsWithOneNode(string newDirectiory, int mediaItemID, int amountOfMediaItems, XDocument docY, string[] filepath)
        {

            int n = 1;
            int extentID = 5;
            bool ersterUmlauf = true;
            //int width = 640;
            //int height = 640;
            // double duration = 20.934000000000001;
            while (n < filepath.Length)
            {
                Tuple<int, int> getValue = new Tuple<int, int>(GetFramesizeOfVideos(filepath, n).Item1, GetFramesizeOfVideos(filepath, n).Item2);
                int width = getValue.Item1;
                int height = getValue.Item2;
                double duration = GetDurationOfVideos(filepath, n);


                mediaItemID++;
                // Erstellt mit jedem Durchgang ein MediaItem
                XElement doc =
                new XElement(new XElement("MediaItem", new XAttribute("id", mediaItemID), new XAttribute("filePath", filepath[n]), new XAttribute("arWidth", width), new XAttribute("arHeight", height), new XAttribute("duration", duration), new XAttribute("songTitle", ""), new XAttribute("songArtist", ""), new XAttribute("songAlbum", ""), new XAttribute("songCopyrightUrl", ""), new XAttribute("songArtistUrl", ""), new XAttribute("songAudioFileUrl", ""), new XAttribute("stabilizationMode", 0), new XAttribute("mediaItemType", 1)));

                docY.Descendants("MediaItems").FirstOrDefault().AddFirst(doc);
                docY.Save(newDirectiory);
                ersterUmlauf = true;
                MakeOneVideoClipItemWithOneNode(newDirectiory, mediaItemID, extentID, docY, ersterUmlauf);
                extentID++;
                n++;
            }
            if (n == filepath.Length)
            {
                ersterUmlauf = false;
                MakeOneVideoClipItemWithOneNode(newDirectiory, mediaItemID, extentID, docY, ersterUmlauf);
            }

            return filepath.Length;
        }


        // VIDEOCLIP
        private static void MakeOneVideoClipItemWithOneNode(string newDirectiory, int mediaItemID, int extentID, XDocument docY, bool ersterUmlauf)
        {
            if (ersterUmlauf == true)
            {
                mediaItemID -= 1;
            }
            else if (ersterUmlauf == false)
            {
            }

            var createdVideoClips = new XElement(new XElement("VideoClip", new XAttribute("extentID", extentID), new XAttribute("gapBefore", 0), new XAttribute("mediaItemID", mediaItemID), new XAttribute("inTime", 0), new XAttribute("outTime", 0), new XAttribute("speed", 1), new XAttribute("stabilizationMode", 0),
                new XElement("Effects"),
                new XElement("Transitions"),
                new XElement("BoundProperties", new XElement("BoundPropertyBool", new XAttribute("Name", "Mute"), new XAttribute("Value", "false")), new XElement("BoundPropertyInt", new XAttribute("Name", "rotateStepNinety"), new XAttribute("Value", "0")), new XElement("BoundPropertyFloat", new XAttribute("Name", "Volume"), new XAttribute("Value", "1")))));
            docY.Root.Descendants("Extents").FirstOrDefault().Add(createdVideoClips);
            docY.Save(newDirectiory);

        }




        // FALSCH? ? ? ? ? ? ? ?



        private static void MakeExtentSelectorForAutomation(string newDirectiory)
        {
            XDocument doc = XDocument.Load(newDirectiory);

            var createdExtentSelector1 = new XElement(new XElement("ExtentSelector", new XAttribute("extentID", 1), new XAttribute("gapBefore", 0), new XAttribute("primaryTrack", "true"), new XElement("Effects"), new XElement("Transitions"), new XElement("BoundProperties"), new XElement("ExtentRefs", "")));
            var createdExtentSelector2 = new XElement(new XElement("ExtentSelector", new XAttribute("extentID", 2), new XAttribute("gapBefore", 0), new XAttribute("primaryTrack", "false"), new XElement("Effects"), new XElement("Transitions"), new XElement("BoundProperties"), new XElement("ExtentRefs")));
            var createdExtentSelector3 = new XElement(new XElement("ExtentSelector", new XAttribute("extentID", 3), new XAttribute("gapBefore", 0), new XAttribute("primaryTrack", "false"), new XElement("Effects"), new XElement("Transitions"), new XElement("BoundProperties"), new XElement("ExtentRefs")));
            var createdExtentSelector4 = new XElement(new XElement("ExtentSelector", new XAttribute("extentID", 4), new XAttribute("gapBefore", 0), new XAttribute("primaryTrack", "false"), new XElement("Effects"), new XElement("Transitions"), new XElement("BoundProperties"), new XElement("ExtentRefs", "")));

            doc.Root.Descendants("Extents").FirstOrDefault().Add(createdExtentSelector1);
            doc.Root.Descendants("Extents").FirstOrDefault().Add(createdExtentSelector2);
            doc.Root.Descendants("Extents").FirstOrDefault().Add(createdExtentSelector3);
            doc.Root.Descendants("Extents").FirstOrDefault().Add(createdExtentSelector4);
            doc.Save(newDirectiory);

        }

        private static void MakeBoundPlaceholders(string newDirectiory)
        {
            XDocument doc = XDocument.Load(newDirectiory);
            var createdBoundPlaceHolders = new XElement(new XElement("BoundPlaceholders", new XElement("BoundPlaceholder", new XAttribute("placeholderID", "SingleExtentView"), new XAttribute("extentID", 0)),
                                                           new XElement("BoundPlaceholder", new XAttribute("placeholderID", "Main"), new XAttribute("extentID", 1)),
                                                           new XElement("BoundPlaceholder", new XAttribute("placeholderID", "SoundTrack"), new XAttribute("extentID", 2)),
                                                           new XElement("BoundPlaceholder", new XAttribute("placeholderID", "Narration"), new XAttribute("extentID", 3)),
                                                           new XElement("BoundPlaceholder", new XAttribute("placeholderID", "Text"), new XAttribute("extentID", 4))));
            doc.Root.Descendants("Extents").FirstOrDefault().AddAfterSelf(createdBoundPlaceHolders);

            doc.Save(newDirectiory);
        }


        private static void MakeBoundProperties(string newDirectiory)
        {
            XDocument doc = XDocument.Load(newDirectiory);

            var createdBoundProperties = new XElement(new XElement("BoundProperties", new XElement("BoundPropertyFloatSet", new XAttribute("Name", "AspectRatio"), new XElement("BoundPropertyFloatElement", new XAttribute("Value", "1.7777776718139648"))), new XElement("BoundPropertyFloat", new XAttribute("Name", "DuckedNarrationAndSoundTrackMix"), new XAttribute("Value", "0.5")),
                                                                                                                                                                                                                                                              new XElement("BoundPropertyFloat", new XAttribute("Name", "DuckedVideoAndNarrationMix"), new XAttribute("Value", "0")),
                                                                                                                                                                                                                                                              new XElement("BoundPropertyFloat", new XAttribute("Name", "DuckedVideoAndSoundTrackMix"), new XAttribute("Value", "0")),
                                                                                                                                                                                                                                                              new XElement("BoundPropertyFloat", new XAttribute("Name", "SoundTrackMix"), new XAttribute("Value", "0"))));


            doc.Root.Descendants("BoundPlaceholders").FirstOrDefault().AddAfterSelf(createdBoundProperties);

            doc.Save(newDirectiory);

        }

        private static void MakeThemeOperationLog(string newDirectiory)
        {
            XDocument doc = XDocument.Load(newDirectiory);

            var createdThemeOperationLog = new XElement(new XElement("ThemeOperationLog", new XAttribute("themeID", 0), new XElement("MonolithicThemeOperations")));

            doc.Root.Descendants("BoundProperties").LastOrDefault().AddAfterSelf(createdThemeOperationLog);

            doc.Save(newDirectiory);

        }


        private static void MakeAudioDuckingProperties(string newDirectiory)
        {
            XDocument doc = XDocument.Load(newDirectiory);

            var audioDuckingProperties = new XElement("AudioDuckingProperties", new XAttribute("emphasisPlaceholderID", "Narration"));
            doc.Root.Descendants("ThemeOperationLog").LastOrDefault().AddAfterSelf(audioDuckingProperties);

            doc.Save(newDirectiory);


        }


        private static int MakeExtentRefForExtentRefs1(string newDirectiory, int letzteZahl, int amountOfMediaItems)
        {
            XDocument doc = XDocument.Load(newDirectiory);

            amountOfMediaItems = GetAmountOfMediaItems(0, doc);

            int n = 0;
            int idIterator = 5;
            int extentID = idIterator + amountOfMediaItems;
            int extent4Id = extentID;
            //string fileName = "";
            while (n < amountOfMediaItems)
            {
                extentID--;

                if (n != amountOfMediaItems)
                {
                    XElement docY = new XElement(
                    new XElement("ExtentRef", new XAttribute("id", extentID)));
                    doc.Descendants("ExtentSelector").Where(i => i.Attribute("extentID").Value == "1").Descendants("ExtentRefs").FirstOrDefault().AddFirst(docY);
                    //getFileNameFromMediaItem(n, docY, reihenFolgeVideo);
                }
                n++;
                doc.Save(newDirectiory);

            }
            return extent4Id;
        }

        private static int GetAmountOfMediaItems(int amountOfMediaItems, XDocument doc)
        {
            IEnumerable<XElement> MediaIDhoechsteZahl = from el in doc.Descendants("MediaItem") where (string)el.Attribute("id") != null select el;
            List<string> MediaElementHoechsteZahl = new List<string>();
            foreach (XElement el in MediaIDhoechsteZahl)
            {
                MediaElementHoechsteZahl.Add(el.FirstAttribute.Value);
                amountOfMediaItems = MediaIDhoechsteZahl.Count();

            }

            return amountOfMediaItems;
        }


        #region Methods not needed at the moment

        //Erstellt ExtentRef(4)
        private static void MakeExtentRefForExtentRef2(string newDirectiory, int extent4ID, int amountOfMediaItems)
        {
            XDocument doc = XDocument.Load(newDirectiory);
            int n = 0;
            extent4ID = extent4ID + amountOfMediaItems;
            while (n < amountOfMediaItems)
            {
                extent4ID--;
                XElement docY = new XElement(
                new XElement("ExtentRef", new XAttribute("id", extent4ID)));
                doc.Descendants("ExtentSelector").Where(i => i.Attribute("extentID").Value == "4").Descendants("ExtentRefs").FirstOrDefault().AddFirst(docY);
                n++;
                doc.Save(newDirectiory);
            }

        }




        private static XElement CreateTitleClipElement(int idIterator, string fileName)
        {
            var currentDirectory = Directory.GetCurrentDirectory() + @"\Slamdank1.wlmp";

            //  double duration = GetDurationOfClip(doc, n);

            XElement titleDocX = new XElement(
                new XElement("TitleClip", new XAttribute("extentID", idIterator), new XAttribute("gapBefore", 0), new XAttribute("duration", 7),
                    new XElement("Effects",                                                                 // Bei Intro = 2 ? ansonsten immer 0 ?
                    new XElement("TextEffect", new XAttribute("effectTemplateID", "TextEffectStretchTemplate"), new XAttribute("TextScriptId", 0),
                    new XElement("BoundProperties",
                    new XElement("BoundPropertyBool", new XAttribute("Name", "automatic"), new XAttribute("Value", "false")),
                    new XElement("BoundPropertyFloatSet", new XAttribute("Name", "color"),
                    new XElement("BoundPropertyFloatElement", new XAttribute("Value", 1)),
                    new XElement("BoundPropertyFloatElement", new XAttribute("Value", 1)),
                    new XElement("BoundPropertyFloatElement", new XAttribute("Value", 1))),
                    new XElement("BoundPropertyStringSet", new XAttribute("Name", "family"),
                    new XElement("BoundPropertyStringElement", new XAttribute("Value", "Rocket Rinder"))),
                    new XElement("BoundPropertyBool", new XAttribute("Name", "horizontal"), new XAttribute("Value", "true")),
                    new XElement("BoundPropertyStringSet", new XAttribute("Name", "justify"),
                    new XElement("BoundPropertyStringElement", new XAttribute("Value", "MIDDLE"))),
                    new XElement("BoundPropertyBool", new XAttribute("Name", "leftToRight"), new XAttribute("Value", "true")),
                    new XElement("BoundPropertyFloatSet", new XAttribute("Name", "length")),
                    new XElement("BoundPropertyFloat", new XAttribute("Name", "maxExtent"), new XAttribute("Value", 0)),
                    new XElement("BoundPropertyFloatSet", new XAttribute("Name", "outlineColor"),
                    new XElement("BoundPropertyFloatElement", new XAttribute("Value", 0)),
                    new XElement("BoundPropertyFloatElement", new XAttribute("Value", 0)),
                    new XElement("BoundPropertyFloatElement", new XAttribute("Value", 0))),
                    new XElement("BoundPropertyInt", new XAttribute("Name", "outlineSizeIndex"), new XAttribute("Value", 1)),
                    new XElement("BoundPropertyFloatSet", new XAttribute("Name", "position"),
                    // HIER SIND DIE POSITIONSVALUES X Y Z
                    new XElement("BoundPropertyFloatElement", new XAttribute("Value", 3)),
                    new XElement("BoundPropertyFloatElement", new XAttribute("Value", 3)),
                    new XElement("BoundPropertyFloatElement", new XAttribute("Value", 3))),
                    new XElement("BoundPropertyFloat", new XAttribute("Name", "size"), new XAttribute("Value", 0.5)),
                    new XElement("BoundPropertyStringSet", new XAttribute("Name", "string"),
                    // HIER IST DIE NAMENS / TEXT VARIABLE
                    new XElement("BoundPropertyStringElement", new XAttribute("Value", "@" + fileName))),
                    new XElement("BoundPropertyString", new XAttribute("Name", "style"), new XAttribute("Value", "Plain")),
                    new XElement("BoundPropertyFloat", new XAttribute("Name", "transparency"), new XAttribute("Value", 0))))),
                    new XElement("Transitions"),
                    new XElement("BoundProperties", new XElement("BoundPropertyFloatSet", new XAttribute("Name", "diffuseColor"),
                    new XElement("BoundPropertyFloatElement", new XAttribute("Value", 0.75)),
                    new XElement("BoundPropertyFloatElement", new XAttribute("Value", 1)),
                    new XElement("BoundPropertyFloatElement", new XAttribute("Value", 0))),
                    new XElement("BoundPropertyFloat", new XAttribute("Name", "transparency"), new XAttribute("Value", 1)))));
            return titleDocX;
        }


        private static string getFileNameFromMediaItem(int n, XDocument doc)
        {

            //string mediaID = GetMediaAndVideoID(n, doc, reihenFolgeVideo);

            // Gibt den Namen des Videos her
            IEnumerable<XElement> MediaFileName = from al in doc.Descendants("MediaItem") where (int)al.Attribute("id") == n + 1 select al;
            IEnumerable<XElement> MediaFilePath = from el in MediaFileName.Descendants("MediaItem") where (string)el.Attribute("filePath") != null select el;
            List<string> MediaItemNameElement = new List<string>();
            foreach (XElement al in MediaFileName)
            {
                MediaItemNameElement.Add(al.Attribute("filePath").Value);
            }
            string mediaItemName = MediaItemNameElement.FirstOrDefault();
            mediaItemName = Path.GetFileName(mediaItemName);
            if (mediaItemName == null)
            {
                return "leer";
            }
            return mediaItemName;
        }

        private static int GetHighestIDNumber(int letzteZahl, IEnumerable<XElement> ExtentRefVideo, List<string> ExtentRefElementVideo)
        {
            foreach (XElement el in ExtentRefVideo)
            {

                ExtentRefElementVideo.Add(el.FirstAttribute.Value);
                if (letzteZahl == 0)
                {
                    letzteZahl = Int32.Parse(el.FirstAttribute.Value);

                }
                int ausgewählteZahl = Int32.Parse(el.FirstAttribute.Value);
                if (ausgewählteZahl >= letzteZahl)
                {
                    letzteZahl = ausgewählteZahl;
                }
            }

            return letzteZahl + 1;
        }

        private static void CreateMediaClipXElement()
        {
            int width = 1920;
            int height = 1080;
            double duration = 30.93;
            string filepath = "";
            new XElement(new XElement("MediaItem", new XAttribute("id", 1), new XAttribute("filePath", filepath), new XAttribute("arWidth", width), new XAttribute("arHeight", height), new XAttribute("duration", duration), new XAttribute("songTitle", ""), new XAttribute("songArtist", ""), new XAttribute("songAlbum", ""), new XAttribute("songCopyrightUrl", ""), new XAttribute("songArtistUrl", ""), new XAttribute("songAudioFileUrl", ""), new XAttribute("stabilizationMode", 0), new XAttribute("mediaItemType", 1)));

        }

        private static void InsertTitleClipAndSave(int lastExtentRefNumber, string newDirectiory, string checkedFileName, int amountOfMediaItems)
        {
            XDocument doc = XDocument.Load(newDirectiory);
            int n = 0;
            while (n < amountOfMediaItems)
            {
                checkedFileName = getFileNameFromMediaItem(n, doc);

                var createdTitleClips = CreateTitleClipElement(lastExtentRefNumber, checkedFileName);
                doc.Root.Descendants("Extents").FirstOrDefault().AddFirst(createdTitleClips);
                doc.Save(newDirectiory);
                n++;
                lastExtentRefNumber++;
            }
        }

        private static string[] GetAllVideosFromFolder()
        {
            string filepath = @"C:\Users\Agrre\Desktop\testproject";
            string[] fileNames;
            fileNames = Directory.GetFiles(filepath, "*.mp4");
            return fileNames;
        }

        private static List<string> GetAllVideosFromFolders()
        {
            string filepath = @"C:\Users\Agrre\Desktop\testproject";
            string[] filePath;
            List<string> lfn = new List<string>();
            filePath = Directory.GetFiles(filepath, "*.mp4");
            foreach (string file in filePath)
            {
                // lfn.Add(Path.GetFileNameWithoutExtension(file));
                lfn.Add(Path.GetFileName(file));
            }
            //  fileNames = new DirectoryInfo(filePath).GetFiles().Select(o => o.Name).ToArray();
            return lfn;
        }


        #endregion
    }
}
