using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Xml.Serialization;
using Scudetti.Helper;
using System.Linq;

namespace Scudetti.Model
{
    public static class ShieldService
    {
        const string ShieldFileName = "ShieldsV3.xml";
        static readonly IsolatedStorageFile _storage = IsolatedStorageFile.GetUserStoreForApplication();
        static readonly Uri _xapUrl = new Uri("Scudetti;component/Data/" + ShieldFileName, UriKind.Relative);

        static XmlSerializer _xml { get { return new XmlSerializer(typeof(Shield[])); } }

        public static void Save(IEnumerable<Shield> scudetti)
        {
            using (var stream = _storage.CreateFile("Shields.xml"))
            {
                _xml.Serialize(stream, scudetti);
            }
        }

        public static IEnumerable<Shield> Load()
        {
#if !DEBUG
            if (_storage.FileExists(ShieldFileName))
            {
                using (var stream = _storage.OpenFile(ShieldFileName, FileMode.Open))
                {
                    return _xml.Deserialize(stream) as Shield[];
                }
            }

            if (_storage.FileExists("ShieldsV2.xml"))
                return Upgrade("ShieldsV2.xml");

            if (_storage.FileExists("Shields.xml"))
                return Upgrade("Shields.xml");
#endif
            return GetNew();
        }

        public static IEnumerable<Shield> GetNew()
        {
            using (var stream = Application.GetResourceStream(_xapUrl).Stream)
            {
                var shields = _xml.Deserialize(stream) as Shield[];
                shields.Shuffle();
                return shields;
            }
        }

        private static IEnumerable<Shield> Upgrade(string oldFile)
        {
            Shield[] oldShields;
            using (var stream = _storage.OpenFile(oldFile, FileMode.Open))
            {
                oldShields = _xml.Deserialize(stream) as Shield[];
            }

            var newShields = GetNew();

            foreach (var shield in oldShields.Where(s => s.IsValidated))
            {
                newShields.Single(s => s.Id == shield.Id).IsValidated = true;
            }

            _storage.DeleteFile(oldFile);

            return newShields;
        }
    }
}