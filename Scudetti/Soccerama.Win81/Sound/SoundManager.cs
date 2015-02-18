using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace Soccerama.Win81.Sound
{
    public static class SoundManager
    {
        private static Random rnd = new Random();
        private static StorageFolder SoundFolder;

        private static MediaElement _fischiettoSound;
        public static async void PlayFischietto()
        {
            //Il fischietto è la prima cosa che viene riprodotta, per cui viene creata qui la cartella
            if (SoundFolder == null)
                SoundFolder = await Package.Current.InstalledLocation.GetFolderAsync("Sound");

            if (!AppContext.SoundEnabled) return;

            if (_fischiettoSound == null)
                _fischiettoSound = await GetSound("fischietto.wav");

            _fischiettoSound.Play();
        }

        private static MediaElement[] _kicksSounds;
        public static async void PlayKick()
        {
            if (!AppContext.SoundEnabled) return;

            if (_kicksSounds == null)
            {
                var tasks = Enumerable.Range(1, 6).Select(i =>
                    GetSound("soccer" + i + ".wav"));
                _kicksSounds = await Task.WhenAll<MediaElement>(tasks);
            }           

            _kicksSounds[rnd.Next(_kicksSounds.Length)].Play();
        }

        private static MediaElement _goalSound;
        public static async void PlayGoal()
        {
            if (!AppContext.SoundEnabled) return;

            if (_goalSound == null)
                _goalSound = await GetSound("goal.wav");

            _goalSound.Play();
        }

        private static MediaElement _boohSound;
        public static async void PlayBooh()
        {
            if (!AppContext.SoundEnabled) return;

            if (_boohSound == null)
                _boohSound = await GetSound("booh.wav");

            _boohSound.Play();
        }

        private static MediaElement _validatedSound;
        public static async void PlayValidated()
        {
            if (!AppContext.SoundEnabled) return;

            if (_validatedSound == null)
                _validatedSound = await GetSound("validated.wav");

            _validatedSound.Play();
        }


        private static async Task<MediaElement> GetSound(string fileName)
        {
            var storageFile = await SoundFolder.GetFileAsync(fileName);
            var stream = await storageFile.OpenAsync(FileAccessMode.Read);
            var me = new MediaElement();
            me.SetSource(stream, storageFile.ContentType);
            return me;
        }

    }
}
