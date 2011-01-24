using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Globalization;

namespace PayMe
{
    public class CurrencyTextConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var currency = (double?)value;
            if (!currency.HasValue) currency = 0.0;
            return string.Format("{0:0.00} {1}", currency, Settings.CurrencySymbol);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var currency = (string)value;
            //currency = currency.Replace(" " + Settings.CurrencySymbol, string.Empty);
            double temp;
            return double.TryParse(currency, NumberStyles.Currency, culture, out temp) ? temp : 0.0;
        }
    }


}
