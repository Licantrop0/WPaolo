using System;
using System.Globalization;
using Windows.UI;
using Windows.UI.Xaml.Data;

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

    public class ColorInverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var color = (Color)value;
            return Color.FromArgb(255, (byte)(255 - color.R), (byte)(255 - color.G), (byte)(255 - color.B));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}