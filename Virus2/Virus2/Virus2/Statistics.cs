using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Virus
{
    public static class Statistics
    {
        public static int Hit = 0;
        public static int Tap = 0;
        public static int BonusPointsGenerated = 0;
        public static int BonusPointsTaken = 0;
        public static int LifesLost = 0;
        public static int BombsUsed = 0;

        public static float HitPrecision 
        {
            get { return (float)Hit / (float)Tap; }  
        }

        public static float BonusPointsRatio
        {
            get { return (float)BonusPointsTaken / (float)BonusPointsGenerated; }
        }

        public static void Reset()
        {
            Hit = 0;
            Tap = 0;
            BonusPointsGenerated = 0;
            BonusPointsTaken = 0;
            LifesLost = 0;
            BombsUsed = 0;
        }
    }
}
