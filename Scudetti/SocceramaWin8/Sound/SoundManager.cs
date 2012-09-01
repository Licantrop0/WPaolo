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

                //_fischiettoSound = new MediaElement()
                //{
                //    AutoPlay = false,
                //    Source = new Uri("ms-appx:///Sound/fischietto.wav")
                //};
            }

            _fischiettoSound.Play();
        }

        //private static SoundEffect _goalSound;
        //public static void PlayGoal()
        //{
        //    if (!AppContext.SoundEnabled) return;

        //    if (_goalSound == null)
        //    {
        //        _goalSound = SoundEffect.FromStream(App.GetResourceStream(
        //                new Uri("Sound\\goal.wav", UriKind.Relative)).Stream);
        //    }

        //    _goalSound.Play();
        //}

        //private static SoundEffect _boohSound;
        //public static void PlayBooh()
        //{
        //    if (!AppContext.SoundEnabled) return;

        //    if (_boohSound == null)
        //    {
        //        _boohSound = SoundEffect.FromStream(App.GetResourceStream(
        //                new Uri("Sound\\booh.wav", UriKind.Relative)).Stream);
        //    }

        //    _boohSound.Play();
        //}

        //private static SoundEffect _validatedSound;
        //public static void PlayValidated()
        //{
        //    if (!AppContext.SoundEnabled) return;

        //    if (_validatedSound == null)
        //    {
        //        _validatedSound = SoundEffect.FromStream(App.GetResourceStream(
        //                new Uri("Sound\\validated.wav", UriKind.Relative)).Stream);
        //    }

        //    _validatedSound.Play();
        //}


    }
}
