﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Xna.Framework.Media;
using SgarbiMix.Model;

namespace SgarbiMix.ViewModel
{
    public class PlayButtonsViewModel
    {
        private SoundContainer[] _soundResources;
        public SoundContainer[] SoundResources
        {
            get
            {
                if (_soundResources == null)
                    _soundResources = (from de in SoundsResources.ResourceManager
                                          .GetResourceSet(CultureInfo.CurrentCulture, true, true)
                                          .Cast<DictionaryEntry>()
                                       orderby de.Key
                                       select new SoundContainer(de.Key.ToString(),
                                           (UnmanagedMemoryStream)de.Value)
                                      ).ToArray();
                return _soundResources;
            }
        }

        private IEnumerable<SoundContainer> _suoniCorti;
        public IEnumerable<SoundContainer> SuoniCorti
        {
            get
            {
                if (_suoniCorti == null)
                    _suoniCorti = SoundResources.Where(s => s.Name.Length <= 18);
                return _suoniCorti;
            }
        }

        private IEnumerable<SoundContainer> _suoniLunghi;
        public IEnumerable<SoundContainer> SuoniLunghi
        {
            get
            {
                if (_suoniLunghi == null)
                    _suoniLunghi = SoundResources.Where(s => s.Name.Length > 18);
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
