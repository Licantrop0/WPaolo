using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using Microsoft.Xna.Framework.Media;
using SheldonMix.Model;

namespace SheldonMix.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private SoundContainerMP3[] _soundResources;
        public SoundContainerMP3[] SoundResources
        {
            get
            {
                if (_soundResources == null)
                    _soundResources = (from de in SoundsResources.ResourceManager
                                          .GetResourceSet(CultureInfo.CurrentCulture, true, true)
                                          .Cast<DictionaryEntry>()
                                       orderby de.Key
                                       select new SoundContainerMP3(de.Key.ToString())
                                      ).ToArray();
                return _soundResources;
            }
        }

        private IEnumerable<SoundContainerMP3> _suoniCLAS;
        public IEnumerable<SoundContainerMP3> SuoniCLAS
        {
            get
            {
                if (_suoniCLAS == null)
                    _suoniCLAS = SoundResources.Where(s => s.Category == SoundType.CLAS);
                return _suoniCLAS;
            }
        }

        private IEnumerable<SoundContainerMP3> _suoniTBBT;
        public IEnumerable<SoundContainerMP3> SuoniTBBT
        {
            get
            {
                if (_suoniTBBT == null)
                    _suoniTBBT = SoundResources.Where(s => s.Category == SoundType.TBBT);
                return _suoniTBBT;
            }
        }

        private IEnumerable<SoundContainerMP3> _suoniZAZZ;
        public IEnumerable<SoundContainerMP3> SuoniZAZZ
        {
            get
            {
                if (_suoniZAZZ == null)
                    _suoniZAZZ = SoundResources.Where(s => s.Category == SoundType.ZAZZ);
                return _suoniZAZZ;
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

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}