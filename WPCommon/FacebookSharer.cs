using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Text;

namespace WPCommon
{
    public class FacebookSharer
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Uri Url { get; set; }
        public Uri ImageUrl { get; set; }

        private const string FbBaseUrl = "http://www.facebook.com/share.php?u=";
        private const string SharerBaseUrl = "http://fb-share-control.com/";       
        
        public FacebookSharer(string title, string description, Uri url, Uri imageUrl)
        {
            Title = title.Replace(' ', '+');
            Description = description.Replace(' ', '+');
            Url = url;
            ImageUrl = imageUrl;
        }

        public string GetShareLink()
        {
            //http://fb-share-control.com/?u=[URL]&amp;t=[TITLE]&amp;i=[IMG]&amp;d=[DESCDRIPTION]

            var queryString = new StringBuilder(SharerBaseUrl);
            queryString.Append("?u=");
            queryString.Append(Url.ToString());
            queryString.Append("&amp;t=");
            queryString.Append(Title);
            queryString.Append("&amp;i=");
            queryString.Append(ImageUrl.ToString());
            queryString.Append("&amp;d=");
            queryString.Append(Description);
            return FbBaseUrl + queryString.ToString();
        }
    }
}
