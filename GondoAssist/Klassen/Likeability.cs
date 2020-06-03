using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GondoAssist.Klassen
{
    class LikeabilityQuellen
    {
        public double Likeability { get; set; }
        public string Link { get; set; }
        public LikeabilityQuellen(double likes, string link)
        {
            this.Likeability = likes;
            this.Link = link;
        }

    }
}
