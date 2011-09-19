using System;
using Microsoft.Xna.Framework.Media;
using System.Windows.Input;
using System.Windows;
using WPCommon.Helpers;

namespace SheldonMix.Model
{
    public enum SoundType
    {
        CLAS,
        TBBT,
        ZAZZ
    }

    public class SoundContainerMP3
    {
        public string Name { get; private set; }
        public SoundType Category { get; private set; }
        private string _rawName;

        public SoundContainerMP3(string rawName)
        {
            _rawName = rawName;

            //Convenzione per classificare: la prime 4 lettere definiscono la classe
            Category = (SoundType)Enum.Parse(typeof(SoundType), rawName.Split('_')[0], false);

            //Convenzioni:
            //"_" = spazio
            //"1" = punto esclamativo
            //"2" = punto interrogativo
            Name = rawName.Substring(5).Replace("_", " ").Replace("1", "!").Replace("2", "?");
        }

        RelayCommand _playCommand;
        public ICommand PlayCommand
        {
            get
            {
                return _playCommand ?? (_playCommand = new RelayCommand(param => Play()));
            }
        }

        public void Play()
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

            var sound = Song.FromUri(_rawName, new Uri("sounds/" + _rawName + ".mp3", UriKind.Relative));
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
