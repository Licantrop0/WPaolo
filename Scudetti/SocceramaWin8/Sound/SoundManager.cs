using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace SocceramaWin8.Sound
{
    public static class SoundManager
    {
        private static Random rnd = new Random();
        private static StorageFolder installedLocation = Windows.ApplicationModel.Package.Current.InstalledLocation;

        private static MediaElement[] _kicksSound;
        public static async void PlayKick()
        {
            if (!AppContext.SoundEnabled) return;

            //var storageFile = await installedLocation.GetFileAsync("Assets\\Sounds\\lazer.mp3");
            //if (storageFile != null)
            //{
            //    var stream = await storageFile.OpenAsync(Windows.Storage.FileAccessMode.Read);
            //    MediaElement snd = new MediaElement();
            //    snd.SetSource(stream, storageFile.ContentType);
            //    snd.Play();
            //}



            //if (_kicksSound == null)
            //{
            //    var storageFiles = await installedLocation.(
            //    _kicksSound = Enumerable.Range(1, 6).Select(i => new MediaElement()
            //    { AutoPlay = false, Source = new Uri("ms-appx:///Sound/soccer" + i + ".wav") })
            //    .ToArray();
            //}

            //_kicksSound[rnd.Next(_kicksSound.Length)].Play();
        }

        private static MediaElement _fischiettoSound;
        public static async void PlayFischietto()
        {
            if (!AppContext.SoundEnabled) return;

            if (_fischiettoSound == null)
            {
                var storageFile = await installedLocation.GetFileAsync("Sound\\fischietto.wav");
                var stream = await storageFile.OpenAsync(FileAccessMode.Read);
                _fischiettoSound = new MediaElement();
                _fischiettoSound.SetSource(stream, storageFile.ContentType);
            }

            _fischiettoSound.Play();
        }

        private static MediaElement _goalSound;
        public static async void PlayGoal()
        {
            if (!AppContext.SoundEnabled) return;

            if (_goalSound == null)
            {
                var storageFile = await installedLocation.GetFileAsync("Sound\\goal.wav");
                var stream = await storageFile.OpenAsync(FileAccessMode.Read);
                _goalSound = new MediaElement();
                _goalSound.SetSource(stream, storageFile.ContentType);
            }

            _goalSound.Play();
        }

        private static MediaElement _boohSound;
        public static async void PlayBooh()
        {
            if (!AppContext.SoundEnabled) return;

            if (_boohSound == null)
            {
                var storageFile = await installedLocation.GetFileAsync("Sound\\booh.wav");
                var stream = await storageFile.OpenAsync(FileAccessMode.Read);
                _boohSound = new MediaElement();
                _boohSound.SetSource(stream, storageFile.ContentType);
            }

            _boohSound.Play();
        }

        private static MediaElement _validatedSound;
        public static async void PlayValidated()
        {
            if (!AppContext.SoundEnabled) return;

            if (_validatedSound == null)
            {
                var storageFile = await installedLocation.GetFileAsync("Sound\\validated.wav");
                var stream = await storageFile.OpenAsync(FileAccessMode.Read);
                _validatedSound = new MediaElement();
                _validatedSound.SetSource(stream, storageFile.ContentType);
            }

            _validatedSound.Play();
        }


    }
}
