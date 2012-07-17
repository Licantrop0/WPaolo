﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Xml.Serialization;
using Scudetti.Helper;

namespace Scudetti.Model
{
    public static class ShieldService
    {
        static readonly IsolatedStorageFile _storage = IsolatedStorageFile.GetUserStoreForApplication();
        static readonly XmlSerializer _xml = new XmlSerializer(typeof(Shield[]));
        static readonly Uri _xapUrl = new Uri("Scudetti;component/Data/Shields.xml", UriKind.Relative);

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
            if (_storage.FileExists("Shields.xml"))
            {
                using (var stream = _storage.OpenFile("Shields.xml", FileMode.Open))
                {
                    return _xml.Deserialize(stream) as Shield[];
                }
            }
#endif

            using (var stream = Application.GetResourceStream(_xapUrl).Stream)
            {
                var shields = _xml.Deserialize(stream) as Shield[];
                shields.Shuffle();
                return shields;
            }
        }
    }
}
