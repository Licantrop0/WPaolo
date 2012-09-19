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
        static StorageFolder roamingFolder = ApplicationData.Current.RoamingFolder;
        static XmlSerializer serializer = new XmlSerializer(typeof(Shield[]));

        public static async Task Save(IEnumerable<Shield> scudetti)
        {
            try
            {
                var file = await roamingFolder.CreateFileAsync("Shields.xml", CreationCollisionOption.ReplaceExisting);
                using (var stream = await file.OpenStreamForWriteAsync())
                {
                    serializer.Serialize(stream, scudetti);
                    stream.Flush();
                }
            }
            catch (IOException)
            {
                throw;
            }
        }

        public static async Task<IEnumerable<Shield>> Load()
        {
#if DEBUG
            return await GetNew();
#else
            if (await FileExist("Shields.xml"))
            {
                var file = await roamingFolder.GetFileAsync("Shields.xml");
                using (var stream = await file.OpenStreamForReadAsync())
                {
                    var obj = (Shield[])serializer.Deserialize(stream);
                    stream.Flush();
                    return obj;
                }
            }
            else
            {
                return await GetNew();
            }
#endif
        }


        private static async Task<bool> FileExist(string fileName)
        {
            try
            {
                await roamingFolder.GetFileAsync(fileName);
                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
            catch (IOException)
            {
                //Unable to load contents of file
                throw;
            }
        }

        public static async Task<IEnumerable<Shield>> GetNew()
        {
            string myFile = Path.Combine(Package.Current.InstalledLocation.Path, "Data\\Shields.xml");
            var myFolder = await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(myFile));

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