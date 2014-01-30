using GalaSoft.MvvmLight;
using Microsoft.Xna.Framework.Media;
using SgarbiMix.WP7.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace SgarbiMix.WP7.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private IEnumerable<LLSGroup<string, SoundViewModel>> _sounds;
        public IEnumerable<LLSGroup<string, SoundViewModel>> Sounds
        {
            get
            {
                if (AppContext.AllSound == null) return null;
                if(_sounds == null)
                    _sounds = from s in AppContext.AllSound
                              group s by s.Category into g
                              select new LLSGroup<string, SoundViewModel>(g);
                return _sounds;
            }
            private set { _sounds = value; }
        }

        public MainViewModel()
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
                return;
            }

            MessengerInstance.Register<string>(this, m =>
            {
                if (m == "update_completed")
                {
                    AppContext.LoadSounds();
                    _sounds = null;
                    RaisePropertyChanged("Sounds");
                }
            });
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
    }
}
