using System.IO;
using Microsoft.Xna.Framework.Audio;

namespace SgarbiMix.Model
{
    public class SoundContainer
    {
        public string Name { get; set; }
        UnmanagedMemoryStream _rawSound;

        private SoundEffect _sound;
        private SoundEffect Sound
        {
            get
            {
                if (_sound == null)
                    _sound = SoundEffect.FromStream(_rawSound);
                return _sound;
            }
        }

        public SoundContainer(string rawName, UnmanagedMemoryStream rawSound)
        {
            //Convenzione: "_" = spazio, "1" = punto esclamativo
            Name = rawName.Replace("_", " ").Replace("1", "!");
            _rawSound = rawSound;
        }

        public bool Play()
        {
            return Sound.Play();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
