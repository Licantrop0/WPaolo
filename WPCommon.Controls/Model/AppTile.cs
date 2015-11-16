using System;
using System.Collections.Generic;

namespace WPCommon.Controls.Model
{
    public class AppTile
    {
        public string name { get; set; }
        public string imageDefault { get; set; }
        public Uri Thumbnail => new Uri(imageDefault);
        public string imageSmall { get; set; }
        public string imageLarge { get; set; }
        public string link { get; set; }
        public string Id => link.Split('=')[1];
        public double rating { get; set; }
        public string reviews { get; set; }
        public string cost { get; set; }
        public string displayPrice { get; set; }
        public string version { get; set; }
        public string releaseDate { get; set; }
        public string lastUpdated { get; set; }
    }

    public class AppList
    {
        public List<AppTile> applications { get; set; }
        public int totalApplications { get; set; }
        public string publisher { get; set; }
        public string countryCode { get; set; }
        public string lastUpdated { get; set; }
    }
}
