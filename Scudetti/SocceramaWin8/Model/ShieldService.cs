using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Scudetti.Model
{
    public static class ShieldService
    {
        static ApplicationData appData = Windows.Storage.ApplicationData.Current;
        static XmlSerializer serializer = new XmlSerializer(typeof(Shield[]));

        public static async void Save(IEnumerable<Shield> scudetti)
        {
            var file = await appData.RoamingFolder.CreateFileAsync("Shields.xml", CreationCollisionOption.ReplaceExisting);

            using (var stream = await file.OpenStreamForWriteAsync())
            {
                serializer.Serialize(stream, scudetti);
                stream.Flush();
            }
        }

        public static async Task<IEnumerable<Shield>> Load()
        {
#if !DEBUG
            try
            {
                var file = await appData.RoamingFolder.GetFileAsync("Shields.xml");

                using (var stream = await file.OpenStreamForReadAsync())
                {
                    var obj = (Shield[])serializer.Deserialize(stream);
                    stream.Flush();
                    return obj;
                }
            }
            catch (FileNotFoundException)
            {
                return GetNew();
            }
            catch (IOException)
            {
                //Unable to load contents of file
                throw;
            }
#else
            return await GetNew();
#endif

        }

        public static async Task<IEnumerable<Shield>> GetNew()
        {
            string myFile = Path.Combine(Package.Current.InstalledLocation.Path, "Data\\Shields.xml");
            StorageFolder myFolder = await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(myFile));

            using (var stream = await myFolder.OpenStreamForReadAsync(Path.GetFileName(myFile)))
            {
                return (Shield[])serializer.Deserialize(stream);

                //var shields = _xml.Deserialize(stream) as Shield[];
                //shields.Shuffle();
                //return shields;
            }
        }
    }
}