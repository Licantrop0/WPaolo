using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TwentyTwelve_Organizer.Model;

namespace TwentyTwelve_Organizer.Helper
{
    public static class ExtensionMethods
    {
        public static bool Between(this double value, double left, double right)
        {
            return value > left && value < right;
        }
    }
}
