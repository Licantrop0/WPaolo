using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace Virus
{
    public static class SoundManager
    {
        public static Dictionary<string, SoundEffect> SoundFXs = new Dictionary<string, SoundEffect>();

        public static void Play(string soundName)
        {
            SoundFXs[soundName].Play();
        }

        /*public static void Stop(string soundName)
        {
            SoundFXs[soundName].
        }*/
    }
}
