using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Xna.Framework.Audio;
using System.IO;

namespace SgarbiMix
{
    public class SoundContainer
    {
        string _name;
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
            _name = rawName.Replace("_", " ").Replace("1", "!");
            _rawSound = rawSound;
        }

        public bool Play()
        {
            return Sound.Play();
        }

        public override string ToString()
        {
            return _name;
        }
    }
}
