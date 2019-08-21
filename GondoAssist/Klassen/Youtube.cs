using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GondoAssist.Klassen
{
    public class Youtube
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Url { get; set; }
        public BitmapImage Thumbnail { get; set; }
        public BitmapImage Image { get; set; }
        public Object SelectedItem { get; set; }

    }
}
