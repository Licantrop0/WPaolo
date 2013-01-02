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
using TwentyTwelve_Organizer;
using TwentyTwelve_Organizer.Model;

namespace TwentyTwelve_Organizer.Helper
{
    public class TaskDifficultyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var currentDifficulty = (TaskDifficulty)value;
            switch (currentDifficulty)
            {
                case TaskDifficulty.VerySimple:
                    return new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
                case TaskDifficulty.Simple:
                    return new SolidColorBrush(Color.FromArgb(255, 80, 160, 0));
                case TaskDifficulty.Hard:
                    return new SolidColorBrush(Color.FromArgb(255, 160, 80, 0));
                case TaskDifficulty.VeryHard:
                    return new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                default: //Nortmal
                    return new SolidColorBrush(Color.FromArgb(255, 127, 127, 0));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
