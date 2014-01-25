using Microsoft.Xna.Framework.Media;
using SgarbiMix.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace SgarbiMix.ViewModel
{
    public class PlayButtonsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public IEnumerable<LLSGroup<string, SoundViewModel>> Sounds { get; set; }

        public PlayButtonsViewModel()
        {
            if (DesignerProperties.IsInDesignTool)
            {
                var s = new SoundViewModel[]
                {
                    new SoundViewModel() { Name = "aoiadoaid", Category = "Corti" },
                    new SoundViewModel() { Name = "aoiadoaid", Category = "Corti" },
                    new SoundViewModel() { Name = "aoiadoaid", Category = "Corti" },
                    new SoundViewModel() { Name = "aoiadoaid", Category = "Corti" },
                    new SoundViewModel() { Name = "aoiadoaid", Category = "Corti" },
                    new SoundViewModel() { Name = "aoiadoaid", Category = "Lunghi" },
                    new SoundViewModel() { Name = "aoiadosaf asdfsadsa aid", Category = "Lunghi" },
                    new SoundViewModel() { Name = "aoiadoasfa sdfadf id", Category = "Lunghi" },
                    new SoundViewModel() { Name = "aoiaf asfsaas dfasf ddoaid", Category = "Lunghi" },
                };

                Sounds = from sound in s
                         group sound by sound.Category into g
                         select new LLSGroup<string, SoundViewModel>(g);
            }
            else
            {
                Sounds = from sound in AppContext.AllSound
                         group sound by sound.Category into g
                         select new LLSGroup<string, SoundViewModel>(g);
            }
        }


        public static void PlayBase(string baseName)
        {
            if (AskAndPlayMusic())
            {
                if (MediaPlayer.Queue.Count == 1 && MediaPlayer.Queue.ActiveSong.Name == baseName && MediaPlayer.State == MediaState.Playing)
                    MediaPlayer.Stop();
                else
                {
                    MediaPlayer.Play(Song.FromUri(baseName, new Uri("sounds/" + baseName + ".mp3", UriKind.Relative)));
                    MediaPlayer.IsRepeating = true;
                }
            }
        }

        private static bool AskAndPlayMusic()
        {
            return MediaPlayer.GameHasControl ?
                true :
                MessageBox.Show("Vuoi interrompere la canzone corrente e riprodurre la base su cui mixare le frasi di Sgarbi?",
                    "SgarbiMix", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
        }

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
