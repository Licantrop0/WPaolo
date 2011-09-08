using System;
using Microsoft.Xna.Framework.Media;

namespace SheldonMix.Model
{
    public enum SoundType
    {
        CLAS,
        TBBT,
        ZAZZ
    }

    public class SoundContainerMP3
    {
        private string _name;
        public SoundType Category { get; private set; }
        private string _rawName;

        public SoundContainerMP3(string rawName)
        {
            _rawName = rawName;

            //Convenzione per classificare: la prime 4 lettere definiscono la classe
            Category = (SoundType)Enum.Parse(typeof(SoundType), rawName.Split('_')[0], false);

            //Convenzioni:
            //"_" = spazio
            //"1" = punto esclamativo
            //"2" = punto interrogativo
            _name = rawName.Substring(5).Replace("_", " ").Replace("1", "!").Replace("2", "?");
        }

        public void Play()
        {
            var sound = Song.FromUri(_rawName, new Uri("sounds/" + _rawName + ".mp3", UriKind.Relative));            
            MediaPlayer.Play(sound);

            MediaPlayer.IsRepeating = false;
        }

        public override string ToString()
        {
            return _name;
        }
    }

    //public class SoundContainer
    //{
    //    public SoundType Category { get; private set; }
    //    private SoundEffect _sound;
    //    private string _name;
    //    private UnmanagedMemoryStream _rawSound;

    //    public SoundContainer(string rawName, UnmanagedMemoryStream rawSound)
    //    {
    //        //Convenzione per classificare: la prime tre lettere definiscono la classe
    //        Category = (SoundType)Enum.Parse(typeof(SoundType), rawName.Split('_')[0], false);

    //        //Convenzioni:
    //        //"_" = spazio
    //        //"1" = punto esclamativo
    //        //"2" = punto interrogativo
    //        _name = rawName.Remove(4).Replace("_", " ").Replace("1", "!").Replace("2", "?");
    //        _rawSound = rawSound;
    //    }

    //    public bool Play()
    //    {
    //        if (_sound == null)
    //            _sound = SoundEffect.FromStream(_rawSound);

    //        return _sound.Play();
    //    }

    //    public override string ToString()
    //    {
    //        return _name;
    //    }
    //}
}
