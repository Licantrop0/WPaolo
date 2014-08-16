using SheldonMix.ViewModel;
using System;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SheldonMix
{
    public static class AppContext
    {
        public static XmlSerializer SoundSerializer { get { return new XmlSerializer(typeof(SoundViewModel[])); } }
        static IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();
        public const string XmlPath = "shared/transfers/Sounds.xml";
        public static event EventHandler UpdateUIEvent = delegate {};
        
        //HACK: supporting only ENG and FRA.
        public static string Lang { get { return CultureInfo.CurrentUICulture.Name == "fr-FR" ? "fr-FR" : "en-US"; } }

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

        public static void UpdateUI(object sender)
        {
            LoadSounds();
            UpdateUIEvent(sender, EventArgs.Empty);
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

            var url = new Uri("http://206.72.115.176/SheldonMix/Sounds.xml" + forceDownload);
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
    }
}
