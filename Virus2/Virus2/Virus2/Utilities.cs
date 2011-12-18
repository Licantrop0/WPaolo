using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Virus
{
    public static class Utilities
    {
        public static double RandomDouble(this Random dice, double min, double max)
        {
            return min + dice.NextDouble() * (max - min);
        }
    }
}
