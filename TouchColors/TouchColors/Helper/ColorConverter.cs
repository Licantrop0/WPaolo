using System;
using System.Globalization;
using Windows.UI;

namespace TouchColors.Helper
{
    public static class ColorConverter
    {
        public static Color FromRgb(string rgb)
        {
            var rgbValue = int.Parse(rgb.Replace("#", ""), NumberStyles.HexNumber);
            return FromRgb(rgbValue);
        }

        public static Color FromRgb(int rgb)
        {
            return Color.FromArgb(
                255,
                (byte)((rgb & 0xff0000) >> 0x10),
                (byte)((rgb & 0xff00) >> 8),
                (byte)(rgb & 0xff));
        }
    }

}