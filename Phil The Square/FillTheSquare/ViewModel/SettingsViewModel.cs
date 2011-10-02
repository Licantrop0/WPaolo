using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Windows;
using FillTheSquare.Localization;
using Microsoft.Xna.Framework.Media;
using System;

namespace FillTheSquare.ViewModel
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private static readonly SettingsViewModel _instance = new SettingsViewModel();
        public static SettingsViewModel Instance { get { return _instance; } }
        private SettingsViewModel() { }


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
                }
            }
        }

        public bool MusicEnabled
        {
            get
            {
                if (MusicEnabledWrapper)
                    MusicEnabledWrapper = AskAndPlayMusic();

                return MusicEnabledWrapper;
            }
            set
            {
                MusicEnabledWrapper = value;
                if (!value) StopMusicIfControl();
            }
        }

        public static void StopMusicIfControl()
        {
            if (MediaPlayer.GameHasControl)
                MediaPlayer.Stop();
        }

        public static bool AskAndPlayMusic()
        {
            var canPlayMusic = MediaPlayer.GameHasControl ?
                                true :
                                MessageBox.Show(AppResources.PlayBackgroundMusic,
                                    AppResources.AppName, MessageBoxButton.OKCancel) == MessageBoxResult.OK;

            var isMyMusicPlaying = MediaPlayer.Queue.Count > 0 &&
                MediaPlayer.State == MediaState.Playing &&
                MediaPlayer.Queue.ActiveSong.Name == "PhilBackgroudMusic";

            if (canPlayMusic && !isMyMusicPlaying)
            {
                var BackgroundMusic = Song.FromUri("PhilBackgroudMusic", new Uri("Sounds/musichetta.wma", UriKind.Relative));
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(BackgroundMusic);
            }

            return canPlayMusic;
        }

        public bool SoundEffectsEnabled
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("sound_effects_enabled"))
                    IsolatedStorageSettings.ApplicationSettings["sound_effects_enabled"] = true;

                return (bool)IsolatedStorageSettings.ApplicationSettings["sound_effects_enabled"];
            }
            set
            {
                if (SoundEffectsEnabled != value)
                {
                    IsolatedStorageSettings.ApplicationSettings["sound_effects_enabled"] = value;
                    OnPropertyChanged("SoundEffectsEnabled");
                }
            }
        }

        public bool HintsEnabled
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("hints_enabled"))
                    IsolatedStorageSettings.ApplicationSettings["hints_enabled"] = true;

                return (bool)IsolatedStorageSettings.ApplicationSettings["hints_enabled"];
            }
            set
            {
                if (HintsEnabled != value)
                {
                    IsolatedStorageSettings.ApplicationSettings["hints_enabled"] = value;
                    OnPropertyChanged("HintsEnabled");
                }
            }
        }
    }
}
