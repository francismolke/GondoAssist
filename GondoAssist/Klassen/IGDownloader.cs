using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GondoAssist.Klassen
{
    public class IGDownloader
    {
        string link, name = "";
        public string Link { get; set; }
        public string Name { get; set; }
        public IGDownloader()
        {
            this.link = Link;
            this.name = Name;
        }
    }
}
