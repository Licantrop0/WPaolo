using System;
using System.Windows.Data;
using IDecide.Localization;
using System.Windows;

namespace IDecide.Helpers
{
    public class ResourceTranslatorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var TranslatedString = DefaultChoices.ResourceManager.GetString((string)value ?? string.Empty);
            return string.IsNullOrEmpty(TranslatedString) ? value : TranslatedString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
