﻿using System;
using System.IO;
using System.Windows.Input;
using Microsoft.Xna.Framework.Audio;
using WPCommon.Helpers;

namespace SgarbiMix.Model
{
    public class SoundViewModel
    {
        private INavigationService _navigationService;
        public INavigationService NavigationService
        {
            get
            {
                if (_navigationService == null)
                    _navigationService = new NavigationService();
                return _navigationService;
            }
        }

        public string Name { get; private set; }
        private UnmanagedMemoryStream _rawSound;
        private SoundEffect _sound;
        private SoundEffect Sound
        {
            get
            {
                if (_sound == null)
                    _sound = SoundEffect.FromStream(_rawSound);
                return _sound;
            }
        }

        public SoundViewModel(string rawName, UnmanagedMemoryStream rawSound)
        {
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
            if (TrialManagement.IsTrialMode && TrialManagement.Counter > 4)
            {
                NavigationService.Navigate(new Uri("/View/DemoPage.xaml", UriKind.Relative));
                return false;
            }
            TrialManagement.IncrementCounter();
            return true;
        }
    }
}
