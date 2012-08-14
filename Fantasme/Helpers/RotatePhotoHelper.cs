using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework.Media;
using NascondiChiappe.Model;

namespace NascondiChiappe.Helpers
{
    public class RotatePhotoHelper
    {
        private IsolatedStorageFile isf { get { return IsolatedStorageFile.GetUserStoreForApplication(); } }
        private MediaLibrary library = new MediaLibrary();
        public event RunWorkerCompletedEventHandler CopyToMediaLibraryCompleted;

        private IList<UberPhoto> Photos;
        private int counter;

        public RotatePhotoHelper(IList<UberPhoto> photos)
        {
            Photos = photos;
            counter = 0;
        }

        public void CopyToMediaLibraryAsync()
        {
            try
            {
                foreach (var photo in Photos)
                {
                    //se non è da ruotare passo direttamente lo stream della foto originale
                    if (photo.RotationAngle == 0)
                    {
                        var PhotoStream = isf.OpenFile(DirectoryName + "\\" + photo.Name, FileMode.Open, FileAccess.Read);
                        bw_RunWorkerCompleted(this, new RunWorkerCompletedEventArgs(PhotoStream, null, false));
                    }
                    else
                    {
                        SaveRotatedPhotoAsync(new WriteableBitmap(photo.Bitmap), photo.RotationAngle);
                    }
                }
            }
            catch (IsolatedStorageException ex)
            {
                new RunWorkerCompletedEventArgs(null, ex, false);
            }
        }

        //TODO: da eliminare dopo aver implementato un ExifWriter
        public void SaveRotatedPhotoAsync(WriteableBitmap wbSource, double rotationAngle)
        {
            var bw = new BackgroundWorker();
            bw.DoWork += (sender, e) =>
            {
                e.Result = RotateBitmap(wbSource, rotationAngle);
            };

            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            bw.RunWorkerAsync();
        }

        //727ms (average 4 samples)
        private static MemoryStream RotateBitmap(WriteableBitmap wbSource, double rotationAngle)
        {
            WriteableBitmap wbTarget = null;
            if (rotationAngle % 180 == 0)
                wbTarget = new WriteableBitmap(wbSource.PixelWidth, wbSource.PixelHeight);
            else
                wbTarget = new WriteableBitmap(wbSource.PixelHeight, wbSource.PixelWidth);

            switch (Convert.ToInt32(rotationAngle))
            {
                case 90:
                    for (int x = 0; x < wbSource.PixelWidth; x++)
                        for (int y = 0; y < wbSource.PixelHeight; y++)
                            wbTarget.Pixels[(wbSource.PixelHeight - y - 1) + x * wbTarget.PixelWidth] =
                                wbSource.Pixels[x + y * wbSource.PixelWidth];
                    break;
                case 180:
                    for (int x = 0; x < wbSource.PixelWidth; x++)
                        for (int y = 0; y < wbSource.PixelHeight; y++)
                            wbTarget.Pixels[(wbSource.PixelWidth - x - 1) + (wbSource.PixelHeight - y - 1) * wbSource.PixelWidth] =
                                wbSource.Pixels[x + y * wbSource.PixelWidth];
                    break;
                case 270:
                    for (int x = 0; x < wbSource.PixelWidth; x++)
                        for (int y = 0; y < wbSource.PixelHeight; y++)
                            wbTarget.Pixels[y + (wbSource.PixelWidth - x - 1) * wbTarget.PixelWidth] =
                                wbSource.Pixels[x + y * wbSource.PixelWidth];
                    break;
                default:
                    throw new ArgumentException("Call not allowed with 0 or wrong angle", "rotationAngle");
            }

            var ms = new MemoryStream();
            wbTarget.SaveJpeg(ms, wbTarget.PixelWidth, wbTarget.PixelHeight, 0, 85);
            ms.Position = 0;
            return ms;
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var photoStream = e.Result as Stream;
            library.SavePicture(Guid.NewGuid().ToString(), photoStream);
            photoStream.Close();

            counter++;
            if (counter == Photos.Count)
                CopyToMediaLibraryCompleted.Invoke(this, new RunWorkerCompletedEventArgs(null, null, false));
        }

    }
}
