using System;
using System.IO;
using System.Windows.Input;
using Microsoft.Xna.Framework.Audio;
using WPCommon.Helpers;

namespace SgarbiMix.ViewModel
{
    public class SoundViewModel
    {
        private INavigationService _navigationService;
        private INavigationService NavigationService
        {
            get
            {
                if (_navigationService == null)
                    _navigationService = new NavigationService();
                return _navigationService;
            }
        }

        public string Name { get; private set; }
        public TimeSpan Duration { get { return Sound.Duration; } }

        private UnmanagedMemoryStream _rawSound;
        private SoundEffect _sound;
        private SoundEffect Sound
        {
            get
            {
                if (_sound == null)
                {
                    _sound = SoundEffect.FromStream(_rawSound);
                    _rawSound.Dispose();
                }
                return _sound;
            }
        }


        public SoundViewModel(string rawName, UnmanagedMemoryStream rawSound)
        {
            if (string.IsNullOrEmpty(rawName))
                throw new ArgumentException("the sound name is null or empty", "rawName");

            //Convenzione: "_" = spazio, "1" = punto esclamativo
            Name = rawName.Replace("_", " ").Replace("1", "!");
            _rawSound = rawSound;
        }

        RelayCommand _playCommand;
        public ICommand PlayCommand
        {
            get
            {
                return _playCommand ?? (_playCommand = new RelayCommand(param => Play()));
            }
        }

        private void Play()
        {
            if (!CheckTrial()) return;
            Sound.Play();
        }

        private bool CheckTrial()
        {
            if (TrialManagement.IsTrialMode && TrialManagement.Counter > 8)
            {
                NavigationService.Navigate(new Uri("/View/DemoPage.xaml", UriKind.Relative));
                return false;
            }
            TrialManagement.IncrementCounter();
            return true;
        }
    }
}
