using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SocceramaWin8.Helper;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Scudetti.Model
{
    public static class ShieldService
    {
        const string ShieldFileName = "ShieldsV3.xml";
        static StorageFolder roamingFolder = ApplicationData.Current.RoamingFolder;
        static XmlSerializer serializer = new XmlSerializer(typeof(Shield[]));

        public static async Task Save(IEnumerable<Shield> scudetti)
        {
            try
            {
                var file = await roamingFolder.CreateFileAsync(ShieldFileName, CreationCollisionOption.ReplaceExisting);
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
#if !DEBUG
            if (await FileExist(ShieldFileName))
            {
                var file = await roamingFolder.GetFileAsync(ShieldFileName);
                using (var stream = await file.OpenStreamForReadAsync())
                {
                    var obj = (Shield[])serializer.Deserialize(stream);
                    stream.Flush();
                    return obj;
                }
            }

            if (await FileExist("ShieldsV2.xml"))
                return await Upgrade("ShieldsV2.xml");

            if (await FileExist("Shields.xml"))
                return await Upgrade("Shields.xml");

#endif
            return await GetNew();
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
            string myFile = Path.Combine(Package.Current.InstalledLocation.Path, "Data", ShieldFileName);
            var myFolder = await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(myFile));

            using (var stream = await myFolder.OpenStreamForReadAsync(Path.GetFileName(myFile)))
            {
                var shields = (Shield[])serializer.Deserialize(stream);
                shields.Shuffle();
                return shields;
            }
        }


        private static async Task<IEnumerable<Shield>> Upgrade(string oldFile)
        {
            Shield[] oldShields;
            var file = await roamingFolder.GetFileAsync(oldFile);
            using (var stream = await file.OpenStreamForReadAsync())
            {
                oldShields = (Shield[])serializer.Deserialize(stream);
                stream.Flush();
            }

            var newShields = await GetNew();

            foreach (var shield in oldShields.Where(s => s.IsValidated))
            {
                newShields.Single(s => s.Id == shield.Id).IsValidated = true;
            }

            await file.DeleteAsync();

            return newShields;
        }
    }
}