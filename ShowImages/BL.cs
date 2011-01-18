using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.Xna.Framework.Media;

namespace ShowImages
{
    public static class BL
    {
        static MediaLibrary library = new MediaLibrary();
        static WebClient wc = new WebClient();

        public static void SaveFiles(List<string> sources)
        {
            Queue<String> uris = new Queue<String>(sources);
            wc.AllowReadStreamBuffering = true;
            SaveNextUri(uris);
        }

        private static void SaveNextUri(Queue<String> uris)
        {
            if (uris.Count == 0) return;
            string u = uris.Dequeue();

            wc.OpenReadCompleted += delegate(object sender, OpenReadCompletedEventArgs e)
            {
                if (e.Cancelled || e.Error != null || e.Result == null)
                    return;
                try
                { library.SavePicture(Path.GetFileNameWithoutExtension(u), e.Result); }
                catch (InvalidOperationException) { }
                SaveNextUri(uris);
            };

            wc.OpenReadAsync(new Uri(u));

            //WebRequest webRequest = WebRequest.Create(u);
            //webRequest.BeginGetResponse(asyncResult =>
            //{
            //    var response = webRequest.EndGetResponse(asyncResult);
            //    using (var stream = response.GetResponseStream())
            //    {
            //        if (stream != null)
            //            library.SavePicture(Path.GetFileName(u), stream);
            //    }

            //    SaveNextUri(uris);
            //}, null);
        }

    }
}
