using System;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Windows;
using DeathTimerz.Localization;
using Microsoft.Xna.Framework.Media;

namespace DeathTimerz.Sounds
{
    public class SoundManager : INotifyPropertyChanged
    {
        private static readonly SoundManager instance = new SoundManager();
        private SoundManager() { }
        public static SoundManager Instance { get { return instance; } }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public bool MusicEnabled
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("music_enabled"))
                    IsolatedStorageSettings.ApplicationSettings["music_enabled"] = true;

                var value = (bool)IsolatedStorageSettings.ApplicationSettings["music_enabled"];
                if (value) return AskAndPlayMusic();
                return value;
            }
            set
            {
                if (value) value = AskAndPlayMusic();
                else MediaPlayer.Stop();
                IsolatedStorageSettings.ApplicationSettings["music_enabled"] = value;
                OnPropertyChanged("MusicEnabled");
            }
        }

        public string DisableEnableMusicText
        {
            //TODO: implement localization
            get { return "enabledisable"; }
        }

        public bool AskAndPlayMusic()
        {
            var canPlayMusic = MediaPlayer.GameHasControl ?
                                true :
                                MessageBox.Show(AppResources.PlayBackgroundMusic,
                                    AppResources.AppName, MessageBoxButton.OKCancel) == MessageBoxResult.OK;

            if (canPlayMusic)
            {
                var BackgroundMusic = Song.FromUri("BackgroudMusic", new Uri("Sounds/Music.wma", UriKind.Relative));
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(BackgroundMusic);
            }

            return canPlayMusic;
        }
    }
}
