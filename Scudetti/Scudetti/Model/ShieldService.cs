using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Resources;
using System.Xml.Serialization;
using Scudetti.Helper;

namespace Scudetti.Model
{
    public static class ShieldService
    {
        static IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
        static XmlSerializer xml { get { return new XmlSerializer(typeof(Shield[])); } }

        public static void Save(IEnumerable<Shield> scudetti)
        {
            using (var stream = storage.CreateFile("Shields.xml"))
            {
                xml.Serialize(stream, scudetti);
            }
        }

        public static IEnumerable<Shield> Load()
        {
#if !DEBUG
            if (storage.FileExists("Shields.xml"))
            {
                using (var stream = storage.OpenFile("Shields.xml", FileMode.Open))
                {
                    return xml.Deserialize(stream) as Shield[];
                }
            }
#endif
            using (var fs = new IsolatedStorageFileStream("Shields.xml", FileMode.Create, FileAccess.Write, storage))
            {
                StreamResourceInfo sri = Application.GetResourceStream(
                    new Uri("Scudetti;component/Data/Shields.xml", UriKind.Relative));

                var shields = xml.Deserialize(sri.Stream) as Shield[];
                shields.Shuffle();
                return shields;
            }
        }
    }
}
