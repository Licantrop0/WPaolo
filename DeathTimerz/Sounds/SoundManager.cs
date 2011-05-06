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

        private bool MusicEnabledWrapper
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("music_enabled"))
                    IsolatedStorageSettings.ApplicationSettings["music_enabled"] = true;

                return (bool)IsolatedStorageSettings.ApplicationSettings["music_enabled"];
            }
            set
            {
                if (MusicEnabledWrapper != value)
                {
                    IsolatedStorageSettings.ApplicationSettings["music_enabled"] = value;
                    OnPropertyChanged("MusicEnabled");
                    OnPropertyChanged("EnableDisabledMusicDescription");
                }
            }
        }

        public bool MusicEnabled
        {
            get
            {
                if (MusicEnabledWrapper && MediaPlayer.GameHasControl)
                    return true;
                else if (MusicEnabledWrapper && !MediaPlayer.GameHasControl)
                    MusicEnabledWrapper = AskAndPlayMusic();

                //if (!MediaPlayer.GameHasControl && MediaPlayer.State != MediaState.Playing)
                return MusicEnabledWrapper;
            }
            set
            {
                if (!value && MediaPlayer.GameHasControl)
                    MediaPlayer.Stop();

                MusicEnabledWrapper = value;
            }
        }

        public string EnableDisabledMusicDescription
        { get { return MusicEnabledWrapper ? AppResources.DisableMusic : AppResources.EnableMusic; } }

        public static bool AskAndPlayMusic()
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
