using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using SgarbiMix.ViewModel;
using System;

namespace SgarbiMix
{
    public static class AppContext
    {
        private static SoundViewModel[] _soundResources;
        public static SoundViewModel[] SoundResources
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
                                      ).ToArray();
                return _soundResources;
            }
        }

        private static Random rnd = new Random();
        public static SoundViewModel GetRandomSound()
        {
            return SoundResources[rnd.Next(SoundResources.Length)];
        }
    }
}