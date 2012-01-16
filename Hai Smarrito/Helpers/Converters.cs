using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using NientePanico.ViewModel;

namespace NientePanico.Helpers
{
    public class GroupToForegroundBrushValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var group = value as IGrouping<char, FlagViewModel>;
            if (group == null) return null;

            if (group.Any())
                return new SolidColorBrush(Colors.White);
            else
                return (SolidColorBrush)Application.Current.Resources["PhoneDisabledBrush"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class GroupToBackgroundBrushValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var group = value as IGrouping<char, FlagViewModel>;
            if (group == null) return null;

            if (group.Any())
                return (SolidColorBrush)Application.Current.Resources["PhoneAccentBrush"];
            else
                return (SolidColorBrush)Application.Current.Resources["PhoneChromeBrush"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
