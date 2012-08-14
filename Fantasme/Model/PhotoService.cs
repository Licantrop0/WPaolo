using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;
using ExifLib;
using Microsoft.Xna.Framework.Media;
using NascondiChiappe.ViewModel;
using System.Linq;

namespace NascondiChiappe.Model
{
    public static class PhotoService
    {
        private static IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();

        public static bool AddPhoto(this AlbumViewModel album, string path, Stream photo)
        {
            var Orientation = ExifReader.ReadJpeg(photo, path).Orientation;
            var FileName = Path.GetFileName(path);
            try
            {
                var wb = RotateBitmap(photo, Orientation);
                using (var fs = isf.CreateFile(FileName))
                {
                    wb.SaveJpeg(fs, wb.PixelWidth, wb.PixelHeight, 0, 85);
                    album.Photos.Add(new UberPhoto(FileName, album.Name));
                }
            }
            catch (IsolatedStorageException)
            {
                return false;
            }

            return true;
        }

        public static void RemovePhoto(this AlbumViewModel album, UberPhoto photo)
        {
            isf.DeleteFile(photo.Path);
            album.Photos.Remove(photo);
        }

        public static void RemovePhotos(this AlbumViewModel album, IList<UberPhoto> photos)
        {
            var n = photos.Count;
            for (int i = 0; i < n; i++)
                album.RemovePhoto(photos[0]);
        }

        public static void RemoveAlbum(this AlbumViewModel album)
        {
            RemovePhotos(album, album.Photos);
        }

        public static void MovePhoto(this AlbumViewModel source, AlbumViewModel target, UberPhoto photo)
        {
            source.Photos.Remove(photo);
            photo.Album = target.Name;
            target.Photos.Add(photo);
        }

        public static void MovePhotos(this AlbumViewModel source, AlbumViewModel target, IList<UberPhoto> photos)
        {
            var n = photos.Count;
            for (int i = 0; i < n; i++)
                source.MovePhoto(target, photos[0]);
        }

        public static void CopyToMediaLibrary(IEnumerable<UberPhoto> photos)
        {
            var library = new MediaLibrary();

            foreach (var photo in photos)
            {
                using (var photoStream = isf.OpenFile(photo.Path, FileMode.Open, FileAccess.Read))
                {
                    var fileName = Path.GetFileName(photo.Path);
                    library.SavePicture(Guid.NewGuid().ToString(), photoStream);
                }
            }
        }

        private static WriteableBitmap RotateBitmap(Stream photo, ExifOrientation orientation)
        {
            if (photo == null)
                throw new ArgumentNullException("photo");

            photo.Position = 0;
            var bitmap = new BitmapImage();
            bitmap.SetSource(photo);
            var wbSource = new WriteableBitmap(bitmap);

            WriteableBitmap wbTarget = null;
            switch (orientation)
            {
                case ExifOrientation.TopRight:
                    wbTarget = new WriteableBitmap(wbSource.PixelHeight, wbSource.PixelWidth);
                    for (int x = 0; x < wbSource.PixelWidth; x++)
                        for (int y = 0; y < wbSource.PixelHeight; y++)
                            wbTarget.Pixels[(wbSource.PixelHeight - y - 1) + x * wbTarget.PixelWidth] =
                                wbSource.Pixels[x + y * wbSource.PixelWidth];
                    return wbTarget;

                case ExifOrientation.BottomRight:
                    wbTarget = new WriteableBitmap(wbSource.PixelWidth, wbSource.PixelHeight);
                    for (int x = 0; x < wbSource.PixelWidth; x++)
                        for (int y = 0; y < wbSource.PixelHeight; y++)
                            wbTarget.Pixels[(wbSource.PixelWidth - x - 1) + (wbSource.PixelHeight - y - 1) * wbSource.PixelWidth] =
                                wbSource.Pixels[x + y * wbSource.PixelWidth];
                    return wbTarget;

                case ExifOrientation.BottomLeft:
                    wbTarget = new WriteableBitmap(wbSource.PixelHeight, wbSource.PixelWidth);
                    for (int x = 0; x < wbSource.PixelWidth; x++)
                        for (int y = 0; y < wbSource.PixelHeight; y++)
                            wbTarget.Pixels[y + (wbSource.PixelWidth - x - 1) * wbTarget.PixelWidth] =
                                wbSource.Pixels[x + y * wbSource.PixelWidth];
                    return wbTarget;

                default: //se non devo ruotare, ritorno il sorgente
                    return wbSource;
            }
        }
    }
}
