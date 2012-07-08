using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Resources;
using System.Xml.Serialization;

namespace Scudetti.Model
{
    public static class ShieldService
    {
        static IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();

        public static void Save(IEnumerable<Shield> scudetti)
        {
            using (var stream = storage.CreateFile("Shields.xml"))
            {
                XmlSerializer xml = new XmlSerializer(scudetti.GetType());
                xml.Serialize(stream, scudetti);
            }
        }

        public static IEnumerable<Shield> Load()
        {
            if (!storage.FileExists("Shields.xml"))
            {
                using (var fs = new IsolatedStorageFileStream("Shields.xml", FileMode.CreateNew, FileAccess.Write, storage))
                {
                    StreamResourceInfo sri = Application.GetResourceStream(
                        new Uri("Scudetti;component/Data/Shields.xml", UriKind.Relative));
                    byte[] bytesInStream = new byte[sri.Stream.Length];
                    sri.Stream.Read(bytesInStream, 0, (int)bytesInStream.Length);
                    fs.Write(bytesInStream, 0, bytesInStream.Length);
                }
            }

            using (var stream = storage.OpenFile("Shields.xml", FileMode.Open))
            {
                XmlSerializer xml = new XmlSerializer(typeof(Shield[]));
                return xml.Deserialize(stream) as Shield[];
            }
        }
    }
}
