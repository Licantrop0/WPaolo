using System;
using Microsoft.Xna.Framework.Audio;

namespace IDecide.Sounds
{
    public static class SoundManager
    {
        private static SoundEffectInstance _crowSound;
        public static void PlayCrow()
        {
            if (_crowSound == null)
                _crowSound = SoundEffect.FromStream(App.GetResourceStream(
                    new Uri("Sounds\\crow.wav", UriKind.Relative)).Stream).CreateInstance();

            if (AppContext.SoundEnabled)
                _crowSound.Play();
        }
        public static void StopCrow()
        {
            if (_crowSound == null)
                return;

            _crowSound.Stop();
        }

        private static SoundEffect _dingSound;
        public static void PlayDing()
        {
            if (_dingSound == null)
                _dingSound = SoundEffect.FromStream(App.GetResourceStream(
                    new Uri("Sounds\\ding.wav", UriKind.Relative)).Stream);

            if (AppContext.SoundEnabled)
                _dingSound.Play();
        }

        private static SoundEffect _rapidDingSound;
        public static void PlayRapidDing()
        {
            if (_rapidDingSound == null)
                _rapidDingSound = SoundEffect.FromStream(App.GetResourceStream(
                    new Uri("Sounds\\rapidDing.wav", UriKind.Relative)).Stream);

            if (AppContext.SoundEnabled)
                _rapidDingSound.Play();
        }
    }
}
