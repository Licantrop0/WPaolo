using System;
using Microsoft.Xna.Framework.Audio;

namespace Capra.Sounds
{
    public static class SoundManager
    {
        static SoundEffect _capraSound;
        public static bool PlayCapra()
        {
            if (_capraSound == null)
                _capraSound = SoundEffect.FromStream(App.GetResourceStream(
                    new Uri("Sounds/capra_b.wav", UriKind.Relative)).Stream);

            return _capraSound.Play();
        }

        static SoundEffect _ignoranteComeCapraSound;
        public static bool PlayIgnoranteComeCapra()
        {
            if (_ignoranteComeCapraSound == null)
                _ignoranteComeCapraSound = SoundEffect.FromStream(App.GetResourceStream(
                    new Uri("Sounds/ignorante_come_capra.wav", UriKind.Relative)).Stream);
            
            return _ignoranteComeCapraSound.Play();
        }
    }
}
