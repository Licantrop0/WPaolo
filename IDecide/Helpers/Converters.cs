using System;
using System.Windows.Data;
using IDecide.Localization;

namespace IDecide.Helpers
{
    public class ResourceTranslatorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var TranslatedString = AppResources.ResourceManager.GetString((string)value);
            return string.IsNullOrEmpty(TranslatedString) ? value : TranslatedString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
