using System;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System.Linq;

namespace Scudetti.Sound
{
    public static class SoundManager
    {
        //private static SoundEffectInstance _crowSound;
        //public static void PlayCrow()
        //{
        //    if (_crowSound == null)
        //        _crowSound = SoundEffect.FromStream(App.GetResourceStream(
        //            new Uri("Sounds\\crow.wav", UriKind.Relative)).Stream).CreateInstance();

        //        _crowSound.Play();
        //}
        //public static void StopCrow()
        //{
        //    if (_crowSound == null)
        //        return;

        //    _crowSound.Stop();
        //}

        private static Random rnd = new Random();

        private static SoundEffect[] _kicksSound;
        public static void PlayKick()
        {
            if (_kicksSound == null)
            {
                _kicksSound = Enumerable.Range(1, 6)
                    .Select(i => SoundEffect.FromStream(App.GetResourceStream(
                        new Uri("Sound\\soccer" + i + ".wav", UriKind.Relative)).Stream))
                        .ToArray();
            }

            //if (AppContext.SoundEnabled)                
            _kicksSound[rnd.Next(_kicksSound.Length)].Play();
        }

        private static SoundEffect _fischiettoSound;
        public static void PlayFischietto()
        {
            if (_fischiettoSound == null)
            {
                _fischiettoSound = SoundEffect.FromStream(App.GetResourceStream(
                        new Uri("Sound\\fischietto.wav", UriKind.Relative)).Stream);                        
            }

            _fischiettoSound.Play();
        }

    }
}
