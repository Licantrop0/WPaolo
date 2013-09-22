using System;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Windows;
using DeathTimerz.Localization;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;

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

        public string EnableDisabledMusicDescription
        {
            get
            {
                return MusicEnabledWrapper ?
                    AppResources.DisableMusic :
                    AppResources.EnableMusic;
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
                IsolatedStorageSettings.ApplicationSettings["music_enabled"] = value;
                OnPropertyChanged("MusicEnabled");
                OnPropertyChanged("EnableDisabledMusicDescription");
            }
        }

        public bool MusicEnabled
        {
            get
            {
                if (MediaPlayer.GameHasControl)
                    return MusicEnabledWrapper;
                else
                {
                    if (MusicEnabledWrapper)
                        MusicEnabledWrapper = AskAndPlayMusic();

                    return MusicEnabledWrapper;
                }
            }
            set
            {
                if (MediaPlayer.GameHasControl)
                {
                    MusicEnabledWrapper = value;
                    if (value)
                        AskAndPlayMusic();
                    else
                        MediaPlayer.Stop();
                }
                else
                    MusicEnabledWrapper = AskAndPlayMusic();
            }
        }

        private static bool AskAndPlayMusic()
        {
            var canPlayMusic = MediaPlayer.GameHasControl ?
                                true :
                                MessageBox.Show(AppResources.PlayBackgroundMusic,
                                    AppResources.AppName, MessageBoxButton.OKCancel) == MessageBoxResult.OK;

            if (canPlayMusic)
            {
                var BackgroundMusic = Song.FromUri("BackgroudMusic", new Uri("Sounds/Music.wma", UriKind.Relative));
                MediaPlayer.IsRepeating = true;
                FrameworkDispatcher.Update(); 
                MediaPlayer.Play(BackgroundMusic);
            }

            return canPlayMusic;
        }

        public void RestoreMusicStatus()
        {
            if (!MusicEnabledWrapper) return;

            MusicEnabledWrapper = AskAndPlayMusic();
        }
    }
}
