using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;

namespace WPCommon.Helpers
{
    public static class Persistance
    {
        private static IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();

        public static void SaveFileToIsolatedStorage(string fileName, string folder = "")
        {
            var filePath = Path.Combine(folder, fileName);
            var streamResourceInfo = Application.GetResourceStream(new Uri(filePath, UriKind.Relative));
            if (!isf.DirectoryExists(folder)) isf.CreateDirectory(folder);

            using (var fileStream = new IsolatedStorageFileStream(filePath, FileMode.Create, isf))
            {
                using (var writer = new BinaryWriter(fileStream))
                {
                    var resourceStream = streamResourceInfo.Stream;
                    long length = resourceStream.Length;
                    byte[] buffer = new byte[32];
                    int readCount = 0;
                    using (var reader = new BinaryReader(streamResourceInfo.Stream))
                    {
                        //read file in chunks in order to reduce memory consumption and increase performance
                        while (readCount < length)
                        {
                            int actual = reader.Read(buffer, 0, buffer.Length);
                            readCount += actual;
                            writer.Write(buffer, 0, actual);
                        }
                    }
                }
            }
        }

        public static void DeleteFile(string fileName, string folder = "")
        {
            var filePath = Path.Combine(folder, fileName); 
            isf.DeleteFile(filePath);
        }

    }
}
