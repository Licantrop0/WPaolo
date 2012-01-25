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
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;
using System.IO;
namespace NientePanico.Helpers
{
    public static class PhotoHelper
    {
        private static IsolatedStorageFile isf { get { return IsolatedStorageFile.GetUserStoreForApplication(); } }

        public static bool SavePhoto(string name, BitmapImage bitmap)
        {
            try
            {
                var wb = new WriteableBitmap(bitmap);
                using (var fs = isf.CreateFile(name))
                {
                    wb.SaveJpeg(fs, wb.PixelWidth, wb.PixelHeight, 0, 85);
                }
            }
            catch (IsolatedStorageException)
            {
                return false;
            }
            return true;
        }

        public static BitmapImage GetPhoto(string name)
        {
            var photo = new BitmapImage();

            try
            {
                using (var file = isf.OpenFile(name, FileMode.Open))
                {
                    photo.SetSource(file);
                }
            }
            catch (IsolatedStorageException)
            { /*non posso farci nulla! */ }

            return photo;
        }
    }
}
