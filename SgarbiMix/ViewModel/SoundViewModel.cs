using Microsoft.Xna.Framework.Audio;
using System;
using System.Windows.Input;
using System.Xml.Serialization;
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

        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string File { get; set; }
        [XmlAttribute]
        public string Category { get; set; }

        public double Width
        {
            get { return Name.Length < 18 ? 248 : 468; }
        }

        public TimeSpan Duration { get { return Sound.Duration; } }

        private SoundEffect _sound;
        private SoundEffect Sound
        {
            get
            {
                if (_sound == null)
                {
                    var srinfo = App.GetResourceStream(new Uri("SgarbiMix;component/Sounds/" + File, UriKind.Relative));
                    _sound = SoundEffect.FromStream(srinfo.Stream);
                }
                return _sound;
            }
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
            if (TrialManagement.IsTrialMode && TrialManagement.Counter > 10)
            {
                NavigationService.Navigate(new Uri("/View/DemoPage.xaml", UriKind.Relative));
                return false;
            }
            TrialManagement.IncrementCounter();
            return true;
        }
    }
}
