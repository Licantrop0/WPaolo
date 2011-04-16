using System;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework.Audio;

namespace ScossaFinta
{
    public static class Settings
    {
        public static SoundEffect SuonoScossa;
        public static SoundEffect SuonoPremio;
        public static SoundEffect SuonoThunder;
        public static SoundEffect SuonoThunderRisata;
        public static SoundEffect ZeusParla;

        public static Stats statistics
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("stats"))
                    IsolatedStorageSettings.ApplicationSettings["stats"] = new Stats(0);
                return (Stats)IsolatedStorageSettings.ApplicationSettings["stats"];
            }
            set
            {
                if (statistics != value)
                    IsolatedStorageSettings.ApplicationSettings["stats"] = value;
            }
        }
    }
}