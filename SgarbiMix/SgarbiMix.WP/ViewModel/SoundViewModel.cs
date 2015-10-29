using Microsoft.Xna.Framework.Audio;
using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Input;
using System.Xml.Serialization;
using WPCommon.Helpers;

namespace SgarbiMix.WP.ViewModel
{
    public class SoundViewModel
    {
        const string baseUri = "shared/transfers/";
        static readonly IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();

        private INavigationService _ns;
        private INavigationService NS => _ns ?? (_ns = new NavigationService());

        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string File { get; set; }
        [XmlAttribute]
        public string Category { get; set; }

        public double Width => Name.Length > 16 ? 468 : 248;

        public TimeSpan Duration => Sound.Duration;

        private SoundEffect _sound;
        private SoundEffect Sound
        {
            get
            {
                if (_sound != null) return _sound;

                try
                {
                    using (var file = isf.OpenFile(baseUri + File, FileMode.Open))
                        _sound = SoundEffect.FromStream(file);
                }
                catch (IsolatedStorageException)
                { }
                return _sound;
            }
        }

        RelayCommand _playCommand;
        public ICommand PlayCommand => _playCommand ?? (_playCommand = new RelayCommand(Play));

        private void Play(object param)
        {
            if (!CheckTrial()) return;
            Sound?.Play();
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
