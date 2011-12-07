using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using IDecide.Localization;

namespace IDecide.Helpers
{
    public class ResourceTranslatorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var TranslatedString = DefaultChoices.ResourceManager.GetString((string)value ?? string.Empty);
            return string.IsNullOrEmpty(TranslatedString) ? value : TranslatedString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ByteArrayToBitmapImage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var arr = (byte[])value;
            var img = new BitmapImage();
            img.SetSource(new MemoryStream(arr));
            return img;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}