using MediaToolkit;
using MediaToolkit.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace GondoAssist.Klassen
{
    public class VideoConverter
    {
        public VideoConverter()
        {

        }


        public void RunConversion(List<string> fileLocation, string filename, string speicherOrt, string selectedFormat, bool isAutoConvert)
        {
            // ausgangsdatei
            string preConvFile;
            string oldspeicherOrt = speicherOrt;
            foreach (var file in fileLocation)
            {
                if( speicherOrt == "")
                {
                    speicherOrt = Path.GetDirectoryName(file) + "\\" +  Path.GetFileNameWithoutExtension(file);
                    preConvFile = file;
                }
                else
                {
                    //Path.GetFileNameWithoutExtension(speicherOrt)
                    if (isAutoConvert == true)
                    {
                        
                    speicherOrt += "\\" + Path.GetFileNameWithoutExtension(filename);
                    preConvFile = file + "\\" + filename; ;

                    }
                    else
                    {
                        speicherOrt += "\\" + Path.GetFileNameWithoutExtension(file);
                        preConvFile = file;
                    }
                }                


                // @"C:\Users\Agrre\Desktop\gege\MondoDrag.webm"
                var inputFile = new MediaToolkit.Model.MediaFile { Filename = preConvFile };
                var outputFile = new MediaToolkit.Model.MediaFile { Filename = speicherOrt + selectedFormat };

                using (var engine = new Engine())
                {
                    engine.Convert(inputFile, outputFile);
                }
            speicherOrt = oldspeicherOrt;
            }
        }

    }
}
