using System;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System.Linq;

namespace Scudetti.Sound
{
    public static class SoundManager
    {
        private static Random rnd = new Random();

        private static SoundEffect[] _kicksSound;
        public static void PlayKick()
        {
            if (!AppContext.SoundEnabled) return;

            if (_kicksSound == null)
            {
                _kicksSound = Enumerable.Range(1, 6)
                    .Select(i => SoundEffect.FromStream(App.GetResourceStream(
                        new Uri("Sound\\soccer" + i + ".wav", UriKind.Relative)).Stream))
                        .ToArray();
            }

            _kicksSound[rnd.Next(_kicksSound.Length)].Play();
        }

        private static SoundEffect _fischiettoSound;
        public static void PlayFischietto()
        {
            if (!AppContext.SoundEnabled) return;

            if (_fischiettoSound == null)
            {
                _fischiettoSound = SoundEffect.FromStream(App.GetResourceStream(
                        new Uri("Sound\\fischietto.wav", UriKind.Relative)).Stream);                        
            }

            _fischiettoSound.Play();
        }

        private static SoundEffect _goalSound;
        public static void PlayGoal()
        {
            if (!AppContext.SoundEnabled) return;

            if (_goalSound == null)
            {
                _goalSound = SoundEffect.FromStream(App.GetResourceStream(
                        new Uri("Sound\\goal.wav", UriKind.Relative)).Stream);
            }

            _goalSound.Play();
        }

        private static SoundEffect _boohSound;
        public static void PlayBooh()
        {
            if (!AppContext.SoundEnabled) return;

            if (_boohSound == null)
            {
                _boohSound = SoundEffect.FromStream(App.GetResourceStream(
                        new Uri("Sound\\booh.wav", UriKind.Relative)).Stream);
            }

            _boohSound.Play();
        }

        private static SoundEffect _validatedSound;
        public static void PlayValidated()
        {
            if (!AppContext.SoundEnabled) return;

            if (_validatedSound == null)
            {
                _validatedSound = SoundEffect.FromStream(App.GetResourceStream(
                        new Uri("Sound\\validated.wav", UriKind.Relative)).Stream);
            }

            _validatedSound.Play();
        }


    }
}
