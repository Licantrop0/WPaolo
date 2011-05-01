using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Phone.Shell;
using FillTheSquare.Localization;
using System.IO.IsolatedStorage;
using System.ComponentModel;

namespace FillTheSquare.Sounds
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
                {
                    IsolatedStorageSettings.ApplicationSettings["music_enabled"] = true;
                    AskAndPlayMusic();
                }
                return (bool)IsolatedStorageSettings.ApplicationSettings["music_enabled"];
            }
            set
            {
                if (value)
                    AskAndPlayMusic();
                else
                    MediaPlayer.Stop();

                if (MusicEnabled != value)
                {
                    IsolatedStorageSettings.ApplicationSettings["music_enabled"] = value;
                    OnPropertyChanged("MusicEnabled");
                }
            }
        }

        public void AskAndPlayMusic()
        {
            if (!PhoneApplicationService.Current.State.ContainsKey("can_play_music"))
            {
                PhoneApplicationService.Current.State.Add("can_play_music", true);
                PhoneApplicationService.Current.State["can_play_music"] =
                    MediaPlayer.GameHasControl ?
                    true :
                    MessageBox.Show(AppResources.PlayBackgroundMusic,
                        AppResources.Name, MessageBoxButton.OKCancel) == MessageBoxResult.OK;
            }
            if ((bool)PhoneApplicationService.Current.State["can_play_music"])
            {
                var BackgroundMusic = Song.FromUri("BackgroudMusic", new Uri("Sounds/musichetta.wma", UriKind.Relative));
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(BackgroundMusic);
            }
        }

        #region Sounds LazyLoading

        public static SoundEffect _moveSound;
        public static SoundEffect MoveSound
        {
            get
            {
                if (_moveSound == null)
                    _moveSound = SoundEffect.FromStream(App.GetResourceStream(
                        new Uri("Sounds\\Move.wav", UriKind.Relative)).Stream);
                return _moveSound;
            }
        }

        private static SoundEffect _errorSound;
        public static SoundEffect ErrorSound
        {
            get
            {
                if (_errorSound == null)
                    _errorSound = SoundEffect.FromStream(App.GetResourceStream(
                        new Uri("Sounds\\Error.wav", UriKind.Relative)).Stream);
                return _errorSound;
            }
        }

        private static SoundEffect _undoSound;
        public static SoundEffect UndoSound
        {
            get
            {
                if (_undoSound == null)
                    _undoSound = SoundEffect.FromStream(App.GetResourceStream(
                        new Uri("Sounds\\Undo.wav", UriKind.Relative)).Stream);
                return _undoSound;
            }
        }

        private static SoundEffect _victorySound;
        public static SoundEffect VictorySound
        {
            get
            {
                if (_victorySound == null)
                    _victorySound = SoundEffect.FromStream(App.GetResourceStream(
                        new Uri("Sounds\\Victory.wav", UriKind.Relative)).Stream);
                return _victorySound;
            }
        }

        private static SoundEffect _resetSound;
        public static SoundEffect ResetSound
        {
            get
            {
                if (_resetSound == null)
                    _resetSound = SoundEffect.FromStream(App.GetResourceStream(
                        new Uri("Sounds\\Reset.wav", UriKind.Relative)).Stream);
                return _resetSound;
            }
        }

        private static SoundEffect _startSound;
        public static SoundEffect StartSound
        {
            get
            {
                if (_startSound == null)
                    _startSound = SoundEffect.FromStream(App.GetResourceStream(
                        new Uri("Sounds\\start.wav", UriKind.Relative)).Stream);
                return _startSound;
            }
        }

        private static SoundEffect _ohNoSound;
        public static SoundEffect OhNoSound
        {
            get
            {
                if (_ohNoSound == null)
                    _ohNoSound = SoundEffect.FromStream(App.GetResourceStream(
                        new Uri("Sounds\\OhNo.wav", UriKind.Relative)).Stream);
                return _ohNoSound;
            }
        }

        #endregion
    }
}
