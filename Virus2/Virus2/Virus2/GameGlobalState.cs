using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace Virus
{
    public static class GameGlobalState
    {
        public static int ActualLevel = 0;

        public static int VirusLives = 0;

        public static bool[] LevelsCompleted = new bool[3];
    }
}