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
using System.Windows.Media.Imaging;
using System.IO;
using System.Globalization;

namespace TrovaCAP
{
    public class ByteArrayToImageBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var arr = (byte[])value;
            var img = new BitmapImage();
            var str = new MemoryStream(arr);
            img.SetSource(str);
            var ib = new ImageBrush();
            ib.Stretch = Stretch.Uniform;
            ib.ImageSource = img;
            return ib;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
