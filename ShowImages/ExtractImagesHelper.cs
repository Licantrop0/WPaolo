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
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

namespace ShowImages
{
    public static class ExtractImagesHelper
    {
        public static void ExtractImages(Uri pageUrl, string htmlString)
        {
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(htmlString);

            var ImgLinks = new List<string>();
            foreach (var img in html.DocumentNode.Descendants("img"))
            {
                var src = img.GetAttributeValue("src", string.Empty);

                if (src.Contains("::")) //Google Images
                {
                    src = src.Substring(src.IndexOf("::") + 2);
                    int AndIndex = src.IndexOf("&");
                    src = "http://" + src.Remove(AndIndex == -1 ? src.Length - 1 : AndIndex);
                }

                if (src.EndsWith(".jpg", StringComparison.InvariantCultureIgnoreCase))
                    ImgLinks.Add(FormatImageUrl(pageUrl, src));
            }

            Settings.CurrentImageList = (from anchor in html.DocumentNode.Descendants("a")
                                         let href = anchor.GetAttributeValue("href", string.Empty)
                                         where href.EndsWith(".jpg", StringComparison.InvariantCultureIgnoreCase)
                                         select FormatImageUrl(pageUrl, href))
                         .Union(ImgLinks)
                         .Where(i => !string.IsNullOrEmpty(i))
                         .ToList();
        }

        private static string FormatImageUrl(Uri pageUrl, string url)
        {
            Uri u;
            if (Uri.TryCreate(url, UriKind.Absolute, out u))
                return url;

            var FirstPart = pageUrl.ToString().Remove(pageUrl.ToString().LastIndexOf('/') + 1);

            if (!url.Contains(pageUrl.Host))
                return FirstPart + url;

            if (!url.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase))
                return "http://" + url;

            return string.Empty;
        }

    }
}
