using SgarbiMix.WP7.ViewModel;
using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SgarbiMix.WP7
{
    public static class AppContext
    {
        public static XmlSerializer SoundSerializer { get { return new XmlSerializer(typeof(SoundViewModel[])); } }
        static IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();
        public const string XmlPath = "shared/transfers/Sounds.xml";

        private static SoundViewModel[] _allSound;
        public static SoundViewModel[] AllSound
        {
            get
            {
                if (_allSound == null)
                    LoadSounds();

                return _allSound;
            }
        }

        public static void LoadSounds()
        {
            if (!isf.FileExists(XmlPath)) return;

            using (var file = isf.OpenFile(XmlPath, FileMode.Open))
            {
                try
                {
                    _allSound = SoundSerializer.Deserialize(file) as SoundViewModel[];
                }
                catch (InvalidOperationException)
                {
                    file.Dispose();
                    isf.DeleteFile(XmlPath); //forza nuovamente il download
                }
            }
        }

        static Random rnd = new Random();
        public static SoundViewModel GetRandomSound()
        {
            return AllSound[rnd.Next(AllSound.Length)];
        }

        public static async Task<Stream> GetNewXmlAsync()
        {
            var http = new HttpClient();
            string forceDownload = string.Empty;
#if debug
            forceDownload = "?t=" + DateTime.Now.Millisecond;
#endif
            var url = new Uri("http://206.72.115.176/SgarbiMix/Sounds.xml" + forceDownload);
            try
            {
                var response = await http.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStreamAsync();
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public static void CloseApp()
        {
            throw new Exception("ForceExit");
        }
    }
}
