using System;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using WPCommon;
using Microsoft.Phone.Shell;
using System.Windows;
using FillTheSquare.Localization;

namespace FillTheSquare
{
    public static class Settings
    {
        #region Sounds

        public static void AskAndPlayMusic()
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

        public static bool MusicEnabled
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("music_enabled"))
                    IsolatedStorageSettings.ApplicationSettings["music_enabled"] = true;
                return (bool)IsolatedStorageSettings.ApplicationSettings["music_enabled"];
            }
            set
            {
                if (MusicEnabled != value)
                    IsolatedStorageSettings.ApplicationSettings["music_enabled"] = value;
            }
        }


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

        public static int CurrentGridSize
        {
            get
            {

                if (!PhoneApplicationService.Current.State.ContainsKey("current_grid_size"))
                    PhoneApplicationService.Current.State.Add("current_grid_size", 5);

                return (int)PhoneApplicationService.Current.State["current_grid_size"];
            }
            set
            {
                PhoneApplicationService.Current.State["current_grid_size"] = value;
            }
        }

        public static ObservableCollection<Record> Records
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("records"))
                    IsolatedStorageSettings.ApplicationSettings["records"] = new ObservableCollection<Record>();
                return (ObservableCollection<Record>)IsolatedStorageSettings.ApplicationSettings["records"];
            }
            set
            {
                if (Records != value)
                    IsolatedStorageSettings.ApplicationSettings["records"] = value;
            }
        }

        public static void AddFakeRecords()
        {
            if (Records.Count >= 30) return;

            var rnd = new Random();
            var temp = new List<Record>();
            for (int i = 0; i < 30; i++)
            {
                temp.Add(new Record((i % 2 + 1) * 5,
                    DateTime.Now.AddDays(-rnd.Next(365)).AddHours(-rnd.Next(24)).AddMinutes(-rnd.Next(60)),
                    TimeSpan.FromSeconds(rnd.Next(1000) + 8)) { Name = "Phil" + i });
            }

            Records.AddRange(temp.OrderBy(r => r.Date));
        }

    }
}