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
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace SheldonMix
{
    // SoundContainer per WAV

    public class SoundContainer
    {

        string _name;
        int type;
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
            //Convenzione per classificare: la prime tre lettere definiscono la classe
            if (rawName.StartsWith("CLA")) type = 1;
            else if (rawName.StartsWith("BAZ")) type = 2;
            else type = 3;
            rawName.Remove(3);
            //Convenzione: "_" = spazio, "1" = punto esclamativo  "2" = punto interrogativo
            _name = rawName.Replace("_", " ").Replace("1", "!").Replace("2","?");
            _rawSound = rawSound;
        }

        public bool Play()
        {
            return Sound.Play();

            //MediaPlayer.Play(Song.FromUri(baseName, new Uri("sounds/" + baseName + ".mp3", UriKind.Relative)));
            //MediaPlayer.IsRepeating = true;
        }

        public override string ToString()
        {
            return _name;
        }

        public int Type()
        {
            return type;
        }
    }

    // SoundContainer per MP3

    public class SoundContainerMP3
    {
        string _rawName;
        string _name;
        int _type;

        public SoundContainerMP3(string rawName)
        {
            //Convenzione per classificare: la prime tre lettere definiscono la classe
            if (rawName.StartsWith("CLAS_")) _type = 1;
            else if (rawName.StartsWith("ZAZZ_")) _type = 2;
            else if (rawName.StartsWith("TBBT_")) _type = 3;
            else
                _type = 4;

            //Convenzione: "_" = spazio, "1" = punto esclamativo  "2" = punto interrogativo
            _name = rawName.Replace("_", " ").Replace("1", "!").Replace("2", "?");
            _rawName = rawName;
            }

        public void Play()
        {
            MediaPlayer.Play(Song.FromUri(_rawName, new Uri("sounds/" + _rawName + ".mp3", UriKind.Relative)));
            MediaPlayer.IsRepeating = false;     
        }

        public override string ToString()
        {
            return _name;
        }

        public int Type()
        {
            return _type;
        }
    }

}
