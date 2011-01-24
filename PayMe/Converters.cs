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

    public class ThresholdConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = System.Convert.ToInt32(((TimeSpan)value).TotalSeconds);
            switch (s)
            {
                case 5 * 60:
                    return 1;
                case 10 * 60:
                    return 2;
                case 15 * 60:
                    return 3;
                case 30 * 60:
                    return 4;
                case 60 * 60:
                    return 5;
                default:
                    return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var index = System.Convert.ToInt32((double)value);
            switch (index)
            {
                case 1:
                    return TimeSpan.FromMinutes(5);
                case 2:
                    return TimeSpan.FromMinutes(10);
                case 3:
                    return TimeSpan.FromMinutes(15);
                case 4:
                    return TimeSpan.FromMinutes(30);
                case 5:
                    return TimeSpan.FromMinutes(60);
                default:
                    return TimeSpan.FromSeconds(1);
            }
        }
    }

    public class ThresholdTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var index = System.Convert.ToInt32((double)value);

            switch (index)
            {
                case 1:
                    return string.Format("{0}: {1}", AppResources.Threshold, AppResources.FiveMinutes);
                case 2:
                    return string.Format("{0}: {1}", AppResources.Threshold, AppResources.TenMinutes);
                case 3:
                    return string.Format("{0}: {1}", AppResources.Threshold, AppResources.FifteenMinutes);
                case 4:
                    return string.Format("{0}: {1}", AppResources.Threshold, AppResources.HalfHour);
                case 5:
                    return string.Format("{0}: {1}", AppResources.Threshold, AppResources.OneHour);
                default:
                    return string.Format("{0}: {1}", AppResources.Threshold, AppResources.OneSecond);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
