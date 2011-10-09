using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.Xna.Framework.Media;

namespace SgarbiMix.ViewModel
{
    public class PlayButtonsViewModel
    {

        private const int LenghtSeparator = 18;

        private IEnumerable<SoundViewModel> _suoniCorti;
        public IEnumerable<SoundViewModel> SuoniCorti
        {
            get
            {
                if (_suoniCorti == null)
                    _suoniCorti = AppContext.SoundResources.Where(s => s.Name.Length <= LenghtSeparator);
                return _suoniCorti;
            }
        }

        private IEnumerable<SoundViewModel> _suoniLunghi;
        public IEnumerable<SoundViewModel> SuoniLunghi
        {
            get
            {
                if (_suoniLunghi == null)
                    _suoniLunghi = AppContext.SoundResources.Where(s => s.Name.Length > LenghtSeparator);
                return _suoniLunghi;
            }
        }

        public void PlayBase(string baseName)
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
