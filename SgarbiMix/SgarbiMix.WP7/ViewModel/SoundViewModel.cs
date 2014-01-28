using Microsoft.Xna.Framework.Audio;
using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Input;
using System.Xml.Serialization;
using WPCommon.Helpers;

namespace SgarbiMix.WP7.ViewModel
{
    public class SoundViewModel
    {
        const string baseUri = "shared/transfers/";
        static IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();

        private INavigationService _ns;
        private INavigationService NS
        {
            get
            {
                if (_ns == null)
                    _ns = new NavigationService();
                return _ns;
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
                    using (var file = isf.OpenFile(baseUri + File, FileMode.Open))
                    {
                        _sound = SoundEffect.FromStream(file);
                    }

                return _sound;
            }
        }

        RelayCommand _playCommand;
        public ICommand PlayCommand
        {
            get
            {
                return _playCommand ?? (_playCommand = new RelayCommand(Play));
            }
        }

        private void Play(object param)
        {
            if (!CheckTrial()) return;
            Sound.Play();
        }

        private bool CheckTrial()
        {
            if (TrialManagement.IsTrialMode && TrialManagement.Counter > 10)
            {
                NS.Navigate(new Uri("/View/DemoPage.xaml", UriKind.Relative));
                return false;
            }
            TrialManagement.IncrementCounter();
            return true;
        }
    }
}
