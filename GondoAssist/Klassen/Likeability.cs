

using System.Collections.Generic;

namespace GondoAssist.Klassen
{
    class LikeabilityQuellen : IComparer<LikeabilityQuellen>
    {
        public double Likeability { get; set; }
        public string Link { get; set; }

        public double Duration { get; set; }
        public string ProfileName { get; set; }

        public string CompareProfileName { get; set; }
        public LikeabilityQuellen(double likes, string link, double duration, string profileName)
        {
            this.Likeability = likes;
            this.Link = link;
            this.Duration = duration;
            this.ProfileName = profileName;
        }


            public int Compare(LikeabilityQuellen x, LikeabilityQuellen y)
            {
                int result = 0;
                //first by age
                while (result != -1)
                result = x.ProfileName.CompareTo(y.ProfileName);

                //then name
                //if (result == 0)
                //    result = x.name.CompareTo(y.name);

                ////a third sort
                //if (result == 0)
                //    result = x.location.CompareTo(y.location);

                return result;
            }
        
        //public int CompareTo(LikeabilityQuellen comparePart)
        //{
        //    // A null value means that this object is greater.
        //    if (comparePart == null)
        //        return 1;

        //    else
        //        return this.ProfileName.CompareTo(comparePart.ProfileName);
        //}

        //public int Compare(LikeabilityQuellen x, LikeabilityQuellen y)
        //{
        //    if (x == null || y == null)
        //    {
        //        return 1;
        //    }

        //    return x.CompareTo(y);
        //}
    }
}
