using System;
using Microsoft.Xna.Framework.Media;
using System.Windows.Input;
using System.Windows;
using WPCommon.Helpers;
using Microsoft.Phone.Tasks;

namespace SheldonMix.ViewModel
{
    public enum SoundType
    {
        CLAS,
        TBBT,
        ZAZZ
    }

    public class SoundViewModel
    {
        public string Name { get; private set; }
        public SoundType Category { get; private set; }
        private string _rawName;

        public SoundViewModel(string rawName, SoundType category)
        {
            _rawName = rawName;
            Category = category;

            Name = rawName.Substring(5, rawName.Length - 9) //9 = 5 (_tag) + 4 (.mp3)
                .Replace("_", " ") //"_" = spazio
                .Replace("1", "!") //"1" = punto esclamativo
                .Replace("2", "?"); //"2" = punto interrogativo
        }

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
            WPCommon.Helpers.Persistance.SaveFileToIsolatedStorage(_rawName, "sounds");
            var saveRingtoneTask = new SaveRingtoneTask();
            try
            {
                saveRingtoneTask.Source = new Uri("isostore:/sounds/" + _rawName);
                saveRingtoneTask.DisplayName = Name;
                saveRingtoneTask.Completed += saveRingtoneTask_Completed;
                saveRingtoneTask.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void saveRingtoneTask_Completed(object sender, TaskEventArgs e)
        {
            WPCommon.Helpers.Persistance.DeleteFile(_rawName, "sounds");
        }


        public void PlayAction(object param)
        {
            if (!AskAndPlayMusic())
                return;

            if (Category == SoundType.TBBT &&
                MediaPlayer.State == MediaState.Playing &&
                MediaPlayer.Queue.ActiveSong.Name == _rawName)
            {
                MediaPlayer.Stop();
                return;
            }

            var sound = Song.FromUri(_rawName, new Uri("sounds/" + _rawName, UriKind.Relative));
            MediaPlayer.Play(sound);
        }

        public static bool AskAndPlayMusic()
        {
            return MediaPlayer.GameHasControl ?
                true :
                MessageBox.Show("Do you want to stop your music and hear what Sheldon have to say??",
                    "SheldonMix", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
        }
    }
}
