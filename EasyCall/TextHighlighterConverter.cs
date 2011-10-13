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
using System.Collections.Generic;

namespace EasyCall.Converters
{
    public class TextHighlighterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return "puppi";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private List<Inline> GetColoredText(string text, string searchText)
        {
            if (string.IsNullOrEmpty(text))
                return new List<Inline>();

            var coloredText = new List<Inline>();
            int index = text.IndexOf(searchText);

            if (index < 0) //non ha trovato la stringa, niente colorazione
                coloredText.Add(new Run() { Text = text });
            else
            {
                var str1 = text.Substring(0, index);
                var str2 = text.Substring(index, searchText.Length);
                var str3 = text.Substring(index + searchText.Length);

                coloredText.Add(new Run { Text = str1 });
                coloredText.Add(new Run
                {
                    Text = str2,
                    Foreground = new SolidColorBrush(Colors.Red) //ResourcesHelper.Get<Brush>("PhoneAccentBrush")
                });
                coloredText.Add(new Run() { Text = str3 });
            }
            return coloredText;
        }

    }
}
