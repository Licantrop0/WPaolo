﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace Scudetti.Helper
{
    public class ImagePathConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Concat("/Images/Scudetti/", value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
