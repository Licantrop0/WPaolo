using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using SgarbiMix.ViewModel;
using System;
using System.Collections.Generic;

namespace SgarbiMix
{
    public static class AppContext
    {
        private static List<SoundViewModel> _soundResources;
        public static List<SoundViewModel>  SoundResources
        {
            get
            {
                if (_soundResources == null)
                    _soundResources = (from de in SoundsResources.ResourceManager
                                          .GetResourceSet(CultureInfo.CurrentCulture, true, true)
                                          .Cast<DictionaryEntry>()
                                       orderby de.Key
                                       select new SoundViewModel(de.Key.ToString(),
                                           (UnmanagedMemoryStream)de.Value)
                                      ).ToList();
                return _soundResources;
            }
        }

        private static Random rnd = new Random();
        public static SoundViewModel GetRandomSound()
        {
            return SoundResources[rnd.Next(SoundResources.Count)];
        }
    }
}