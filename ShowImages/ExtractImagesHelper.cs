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
using WPCommon;

namespace ShowImages
{
    public static class ExtractImagesHelper
    {
        public static void ExtractImages(Uri pageUrl, string htmlString)
        {
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(htmlString);

            var ImgLinks = new List<string>();

            //Aggiunge i tag Anchor
            foreach (var anchor in html.DocumentNode.Descendants("a"))
            {
                var href = anchor.GetAttributeValue("href", string.Empty);
                if (href.Contains("/m/imgres?q=")) //Google Images
                {
                    href = href.Replace("&amp;", "&");
                    href = href.Substring(href.IndexOf("imgurl=") + 7);
                    href = href.Remove(href.IndexOf("&imgrefurl="));
                }
                ImgLinks.Add(href);
            }

            //Aggiunge i tag Img
            ImgLinks.Concat(html.DocumentNode.Descendants("img")
                .Select(img => img.GetAttributeValue("src", string.Empty)));

            Settings.CurrentImageList.Clear();

            //Aggiunge le immagini .jpg alla CurrentImageList formattate correttamente
            (from i in ImgLinks
             where i.EndsWith(".jpg", StringComparison.InvariantCultureIgnoreCase)
             let iurl = FormatImageUrl(pageUrl, i)
             where !string.IsNullOrEmpty(iurl)
             select new SelectionableImage(iurl))
             .ForEach(i => Settings.CurrentImageList.Add(i));
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
