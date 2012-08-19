using System;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Data;
using Microsoft.Phone;

namespace NascondiChiappe.Helpers
{
    public class DecodeImageConverter : IValueConverter
    {
        private static IsolatedStorageFile ISF = IsolatedStorageFile.GetUserStoreForApplication();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var path = (string)value;
            try
            {
                using (var sourceFile = ISF.OpenFile(path, FileMode.Open, FileAccess.Read))
                {
                    return PictureDecoder.DecodeJpeg(sourceFile, 200, 200);
                }
            }
            catch (IsolatedStorageException)
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}