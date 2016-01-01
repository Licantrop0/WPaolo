using System.Collections.Generic;
using TouchColors.Model;
using Windows.UI;

namespace TouchColors.DesignMode
{
    public static class DesignData
    {
        public static List<NamedColor> GetColors()
        {
            return new List<NamedColor>
            {
                new NamedColor("Blue", Colors.Blue),
                new NamedColor("Brown", Colors.Brown),
                new NamedColor("Cornsilk", Colors.Cornsilk),
                new NamedColor("Dark Red", Colors.DarkRed),
                new NamedColor("Green", Colors.Green)
            };
        }
    }
}
