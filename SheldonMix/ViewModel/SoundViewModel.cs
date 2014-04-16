using System;
using Microsoft.Xna.Framework.Media;
using System.Windows.Input;
using System.Windows;
using WPCommon.Helpers;
using Microsoft.Phone.Tasks;
using System.Xml.Serialization;
using SheldonMix.Localization;
using System.IO.IsolatedStorage;
using System.IO;

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
            if (!AskAndPlayMusic())
                return;

            if (Category == SoundCategory.TBBT &&
                MediaPlayer.State == MediaState.Playing &&
                MediaPlayer.Queue.ActiveSong.Name == File)
            {
                MediaPlayer.Stop();
                return;
            }

            
            using (var isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var isfs = isf.OpenFile("shared/transfers/" + File, FileMode.Open))
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
