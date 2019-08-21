using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GondoAssist.Klassen
{
    public class Timestamp
    {
        string so;
        string xmlfilepath;
        public Timestamp()
        {
            
            GetXMLInformation(so, xmlfilepath);
            File.CreateText(so + "\\Timestamp.txt");
         //   DeleteExtension();
        }

        private void DeleteExtension()
        {
            using (StreamReader filereader = new StreamReader(so + "\\Timestamp.txt"))
            {
                string fr = filereader.ReadLine();
                fr.Split('-').LastOrDefault().Trim();
                using (StreamWriter file = new StreamWriter(so + "\\Timestamp.txt", true, Encoding.UTF8))
                {
                    file.WriteLine(string.Format(" " + fr));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="so">Speicherort</param>
        /// <param name="xmlfilepath">Quelle der .xml-Datei</param>
        public static void GetXMLInformation(string so, string xmlfilepath)
        {
            // gibt die Startzeit für die Timestamp an
            double start = 0;
            // gibt die Endzeit für die Timestamp an
            double ende = 0;
            // berechnungsvariable für die Timestamps
            double minute;
            // berechnungsvariable für die Timestamps
            double sekunde;
            // while schleifen ########################
            int n = 0;
            // globale variable für media-Duration-double
            double mdD;



            // Erstellt eine leere .txt Datei am gewählten Speicherort
            using (StreamWriter file = new StreamWriter(so + "\\Timestamp.txt", false, Encoding.UTF8))
            {
                file.Write(string.Format(""));
            }

            // In dieser while schleife wird eine XML-Datei ausgelesen,
            //  in eine variable gepackt und später verarbeitet.
            while (n < 100)
            {
                
                double mediaDurDouble;
                
                // Speicherort wird hier verarbeitet
                var filename = xmlfilepath;
                var currentDirectory = Directory.GetCurrentDirectory();
                XDocument doc = XDocument.Load(Path.Combine(currentDirectory, filename));


                // gibt die Reihenfolge der Videos in MovieMaker in eine Liste 
                // the Order of videos in MovieMaker  ExtentID
                IEnumerable<XElement> ExtentID = from el in doc.Descendants("ExtentRef") where (string)el.Attribute("id") != null select el;
                List<string> ExtentElement = new List<string>();
                foreach (XElement el in ExtentID)
                {
                    ExtentElement.Add(el.FirstAttribute.Value);

                }
                //  verpackt die erhaltenen daten in der schleife zugreifbare variablen
                string extentID = ExtentElement.Skip(n).FirstOrDefault();

                // Erhalte die erweiterte ID des Videosclips, um mit mediaItemID arbeiten zu können
                // CacheMemory for Extent- & Media- ID
                IEnumerable<XElement> VideoClipID = from el in doc.Descendants("VideoClip") where (string)el.Attribute("extentID") == extentID select el;
                List<string> VideoClipElement = new List<string>();
                foreach (XElement el in VideoClipID)
                {
                    VideoClipElement.Add(el.FirstAttribute.Value);

                }
                //  verpackt die erhaltenen daten in der schleife zugreifbare variablen
                string VextentID = VideoClipElement.FirstOrDefault();

                // Erhalte die erweiterte ID des Videosclips, um mit mediaItemID arbeiten zu können
                // the Order of videos in MovieMaker  ExtentID
                IEnumerable<XElement> mediaItemID = from al in doc.Descendants("VideoClip") where (string)al.Attribute("extentID") == extentID select al;
                List<string> media_ItemElement = new List<string>();
                foreach (XElement al in mediaItemID)
                {
                    media_ItemElement.Add(al.Attribute("mediaItemID").Value);
                }
                //  verpackt die erhaltenen daten in der schleife zugreifbare variablen
                string VmediaID = media_ItemElement.FirstOrDefault();


                // Die ID für die Filepath
                // PathMemory with Media ID
                IEnumerable<XElement> MediaID = from el in doc.Descendants("MediaItem") where (string)el.Attribute("id") == VmediaID select el;
                List<string> MediaElement = new List<string>();
                foreach (XElement el in MediaID)
                {
                    MediaElement.Add(el.FirstAttribute.Value);
                }

                //  verpackt die erhaltenen daten in der schleife zugreifbare variablen
                string mediaID = MediaElement.FirstOrDefault();

                // Gibt die FilePath aus
                //Media Item filePath
                IEnumerable<XElement> MediaItemFilepath = from al in doc.Descendants("MediaItem") where (string)al.Attribute("id") == mediaID select al;
                List<string> MediaItemFilepathElement = new List<string>();
                foreach (XElement al in MediaItemFilepath)
                {
                    MediaItemFilepathElement.Add(al.Attribute("filePath").Value);
                }

                //  verpackt die erhaltenen daten in der schleife zugreifbare variablen
                string mediafilePath = MediaItemFilepathElement.FirstOrDefault();           
                string mediaFileName = Path.GetFileName(mediafilePath);

                // Gibt die id der Länge des Videos
                IEnumerable<XElement> MediaItemDur = from al in doc.Descendants("MediaItem") where (string)al.Attribute("id") == mediaID select al;
                List<string> MediaItemDurElement = new List<string>();
                foreach (XElement al in MediaItemDur)
                {
                    MediaItemDurElement.Add(al.Attribute("duration").Value);
                }                

                // Gibt die Länge in englischer culture info aus
                string mediaDuration = MediaItemDurElement.FirstOrDefault();
                if (mediaDuration != null)
                {
                    mdD = double.Parse(mediaDuration, NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture("en-GB"));                   
                }
                else
                {
                    break;
                }
                //  verpackt die erhaltenen daten in eine global zugreifbare variable
                mediaDurDouble = mdD;



                // Gibt die ID an ob ein Video gekürzt wurde
                IEnumerable<XElement> VoutTime = from al in doc.Descendants("VideoClip") where (string)al.Attribute("mediaItemID") == mediaID select al;
                List<string> MediaVoutTime = new List<string>();
                foreach (XElement al in VoutTime)
                {
                    MediaVoutTime.Add(al.Attribute("outTime").Value);

                }
                //  verpackt die erhaltenen daten in der schleife zugreifbare variablen
                string videoOutTime = MediaVoutTime.FirstOrDefault();
                double videoOutTDouble = double.Parse(videoOutTime, NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture("en-GB"));


                // Selektiert und gibt in eine Liste in VideoClip alle outTimes mit keinem Wert.
                IEnumerable<XElement> XoutTime = from al in doc.Descendants("VideoClip") where (string)al.Attribute("outTime") != null select al;
                List<string> XediaVoutTime = new List<string>();
                foreach (XElement al in XoutTime)
                {
                    XediaVoutTime.Add(al.Attribute("outTime").Value);

                }
                //  verpackt die erhaltenen daten in der schleife zugreifbare variablen
                string XideoOutTime = XediaVoutTime.FirstOrDefault();
                double XideoOutTDouble = double.Parse(XideoOutTime, NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture("en-GB"));


                // Selektiert und gibt in eine Liste die Videoclips, die geschnitten wurden (gekürzt!=geschnitten)
                IEnumerable<XElement> VinTime = from al in doc.Descendants("VideoClip") where (string)al.Attribute("mediaItemID") == mediaID select al;
                List<string> MediaVinTime = new List<string>();
                foreach (XElement al in VinTime)
                {
                    MediaVinTime.Add(al.Attribute("inTime").Value);

                }

                //  verpackt die erhaltenen daten in der schleife zugreifbare variablen
                string VideoinTime = MediaVinTime.FirstOrDefault();
                double VideoinTimeDouble = double.Parse(VideoinTime, NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture("en-GB"));



                // verpackt die erhaltenen daten in der schleife zugreifbare variablen
                // Rechnet die länge aus für die spätere Ausgabe
                double xresult;
                double plusresult;
                if (mediaDurDouble == videoOutTDouble)
                {
                    xresult = Math.Round(mediaDurDouble);
                    plusresult = xresult;
                }
                else
                {
                    xresult = Math.Round(mediaDurDouble - videoOutTDouble);
                    plusresult = Math.Round(mediaDurDouble) - xresult;
                }


                // Hier wird der Name der Datei festgelegt
                Regex mr1 = new Regex(@"^[a-z._A-Z0-9]+\s-\s.{11}\.mp4$");

                // sortiert nach Reihenfolge
                if (extentID == VextentID)
                {
                    // regelt die weitere Reihenfolge
                    if (VmediaID == mediaID)

                        // prüft ob die Zeit über 60 Sekunden ist und teilt sie in Minuten 
                        if (plusresult > 60)

                        {
                            sekunde = plusresult % 60;
                            minute = Math.Floor(plusresult / 60);
                            // Schreibt die die Timestamps in die Timestamp Textdatei.
                            using (StreamWriter file = new StreamWriter(so + "\\Timestamp.txt", true, Encoding.UTF8))
                            {
                                file.Write(string.Format("{0:00}:{1:00}", ende, start));
                                ende += minute;

                                start += sekunde;
                                if (start > 60)
                                {
                                    start %= 60;
                                    ende += 1;
                                }
                                file.Write(string.Format(" - " + "{0:00}:{1:00}", ende, start));
                                file.WriteLine(string.Format(" " + mediaFileName));


                            }

                        }
                        else if (xresult > 60)
                        {                            
                            sekunde = plusresult % 60;
                            minute = Math.Floor(plusresult / 60);
                            //xresult
                            using (StreamWriter file = new StreamWriter(so + "\\Timestamp.txt", true, Encoding.UTF8))
                            {
                                file.Write(string.Format("{0:00}:{1:00}", ende, start));
                                ende += minute;

                                start += sekunde;
                                if (start > 60)
                                {
                                    start %= 60;
                                    ende += 1;
                                }
                                file.Write(string.Format(" - " + "{0:00}:{1:00}", ende, start));
                                file.WriteLine(string.Format(" " + mediaFileName));
                            }
                        }
                        else if (videoOutTDouble != 0)
                        {
                            if (VideoinTimeDouble != 0)
                            {
                                plusresult -= VideoinTimeDouble;
                                
                            }
                            sekunde = plusresult % 60;
                            minute = Math.Floor(plusresult / 60);


                            using (StreamWriter file = new StreamWriter(so + "\\Timestamp.txt", true, Encoding.UTF8))
                            {
                                file.Write(string.Format("{0:00}:{1:00}", ende, start));
                                ende += minute;

                                start += sekunde;
                                if (start > 60)
                                {
                                    start %= 60;
                                    ende += 1;
                                }
                                file.Write(string.Format(" - " + "{0:00}:{1:00}", ende, start));
                          
                            }

                            if (mr1.IsMatch(mediaFileName))
                                using (StreamWriter file = new StreamWriter(so + "\\Timestamp.txt", true, Encoding.UTF8))
                                {
                                    string xdeq = string.Format(" https://www.instagram.com/p/" + mediaFileName.Substring(mediaFileName.IndexOf('-') + 1).Trim());
                                    int fileExtPos = xdeq.LastIndexOf(".");
                                    if (fileExtPos >= 0)
                                        xdeq = xdeq.Substring(0, fileExtPos);
                                    file.WriteLine(xdeq);
                                }


                            else
                                using (StreamWriter file = new StreamWriter(so + "\\Timestamp.txt", true, Encoding.UTF8))
                                {

                                    file.WriteLine(string.Format(" " + mediaFileName));
                                }
                        }
                        else
                        {
                            sekunde = xresult % 60;
                            minute = Math.Floor(xresult / 60);

                            using (StreamWriter file = new StreamWriter(so + "\\Timestamp.txt", true, Encoding.UTF8))
                            {
                                file.Write(string.Format("{0:00}:{1:00}", ende, start));
                                ende += minute;

                                start += sekunde;
                                if (start > 60)
                                {
                                    start %= 60;
                                    ende += 1;
                                }
                                file.Write(string.Format(" - " + "{0:00}:{1:00}", ende, start));
                            }
                            if (mr1.IsMatch(mediaFileName))
                                using (StreamWriter file = new StreamWriter(so + "\\Timestamp.txt", true, Encoding.UTF8))
                                {
                                    string xdeq = string.Format(" https://www.instagram.com/p/" + mediaFileName.Substring(mediaFileName.IndexOf('-') + 1).Trim());
                                    int fileExtPos = xdeq.LastIndexOf(".");
                                    if (fileExtPos >= 0)
                                        xdeq = xdeq.Substring(0, fileExtPos);
                                    file.WriteLine(xdeq);
                                }
                            else
                                using (StreamWriter file = new StreamWriter(so + "\\Timestamp.txt", true, Encoding.UTF8))
                                {
                                    file.WriteLine(string.Format(" " + mediaFileName));
                                }
                        }


                }
                n++;
            }

        }

    }
}

