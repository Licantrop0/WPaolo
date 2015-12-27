using System.Globalization;
using System.Windows.Media;

namespace TouchColors.Helper
{
    public static class ColorConverter
    {
        public static Color FromRgb(int argb)
        {
            return Color.FromArgb(255,
                                  (byte)((argb & 0xff0000) >> 0x10),
                                  (byte)((argb & 0xff00) >> 8),
                                  (byte)(argb & 0xff));
        }
    }
}