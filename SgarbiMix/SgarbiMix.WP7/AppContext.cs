using SgarbiMix.WP7.ViewModel;
using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
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
                    _allSound = LoadSounds();

                return _allSound;
            }
        }

        private static SoundViewModel[] LoadSounds()
        {
            //Va alla pagina degli update direttamente
            if (!isf.FileExists(XmlPath))
                return null;
            using (var file = isf.OpenFile(XmlPath, FileMode.Open))
            {
                return SoundSerializer.Deserialize(file) as SoundViewModel[];
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
            try
            {
                var response = await http.GetAsync(new Uri("http://206.72.115.176/SgarbiMix/Sounds.xml"));
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
