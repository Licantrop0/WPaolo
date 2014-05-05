using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media;
using SheldonMix.Localization;
using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using WPCommon.Helpers;

namespace SheldonMix.ViewModel
{
    public enum SoundCategory
    {
        CLAS,
        TBBT,
        ZAZZ
    }

    public class SoundViewModel
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public SoundCategory Category { get; set; }
        [XmlAttribute]
        public string File { get; set; }
        [XmlAttribute]
        public string Lang { get; set; }

        RelayCommand _playCommand;
        public ICommand PlayCommand
        {
            get
            {
                return _playCommand ?? (_playCommand = new RelayCommand(PlayAction));
            }
        }

        RelayCommand _setAsRingtoneCommand;
        public ICommand SetAsRingtoneCommand
        {
            get
            {
                return _setAsRingtoneCommand ?? (_setAsRingtoneCommand = new RelayCommand(SetAsRingtoneAction));
            }
        }

        //Ringtone files must be of type MP3 or WMA.
        //Ringtone files must be less than 40 seconds in length.
        //Ringtone files must not have digital rights management (DRM) protection.
        //Ringtone files must be less than 1 MB in size.
        private void SetAsRingtoneAction(object obj)
        {
            try
            {
                new SaveRingtoneTask()
                {
                    Source = new Uri("isostore:/shared/transfers/" + File),
                    DisplayName = Name
                }.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void PlayAction(object param)
        {
            //if (!AskAndPlayMusic())
            //    return;
                
            var me = (MediaElement)param;

            //stops the current sound if more than 5 sec
            if (me.CurrentState == MediaElementState.Playing &&
                me.NaturalDuration.TimeSpan >= TimeSpan.FromSeconds(5) &&
                me.Source.Segments.Last() == File)
            {
                me.Stop();
                return;
            }

            
            using (var isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var isfs = isf.OpenFile("shared/transfers/" + File, FileMode.Open, FileAccess.Read))
                {
                    me.SetSource(isfs);
                }
            }
            me.Play();
        }

        public static bool AskAndPlayMusic()
        {
            return MediaPlayer.GameHasControl ?
                true :
                MessageBox.Show(AppResources.StopMusic,
                    "SheldonMix", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
        }
    }
}
