using Microsoft.Xna.Framework.Audio;
using System;

namespace TwentyTwelve_Organizer.Sound
{
    public static class SoundManager
    {
        private static SoundEffect _tickSound;

        public static SoundEffect TickSound
        {
            get
            {
                if (_tickSound == null)
                {
                    var str = App.GetResourceStream(new Uri("Sound/TickSound.wav", UriKind.Relative));
                    _tickSound = SoundEffect.FromStream(str.Stream);
                }
                return _tickSound;
            }
        }

        private static SoundEffect _buttonUpSound;
        public static SoundEffect ButtonUpSound
        {
            get
            {
                if (_buttonUpSound == null)
                {
                    var str = App.GetResourceStream(new Uri("Sound/ButtonUpSound.wav", UriKind.Relative));
                    _buttonUpSound = SoundEffect.FromStream(str.Stream);
                }
                return _tickSound;
            }
        }

        private static SoundEffect _buttonDownSound;
        public static SoundEffect ButtonDownSound
        {
            get
            {
                if (_buttonDownSound == null)
                {
                    var str = App.GetResourceStream(new Uri("Sound/ButtonDownSound.wav", UriKind.Relative));
                    _buttonDownSound = SoundEffect.FromStream(str.Stream);
                }
                return _tickSound;
            }
        }
    }
}
