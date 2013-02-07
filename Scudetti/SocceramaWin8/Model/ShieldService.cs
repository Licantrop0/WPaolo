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
        private enum StorageType
        {
            Local,
            Roaming
        }

        const string ShieldFileName = "ShieldsV4.xml";
        static StorageFolder roamingFolder = ApplicationData.Current.RoamingFolder;
        static StorageFolder localFolder = ApplicationData.Current.LocalFolder;

        static XmlSerializer serializer = new XmlSerializer(typeof(Shield[]));

        public static async Task Save(IEnumerable<Shield> scudetti)
        {
            try
            {
                var file = await localFolder.CreateFileAsync(ShieldFileName, CreationCollisionOption.ReplaceExisting);
                using (var stream = await file.OpenStreamForWriteAsync())
                {
                    serializer.Serialize(stream, scudetti);
                    stream.Flush();
                }
            }
            catch (IOException)
            {
                //ouch... riproviamo la prossima volta
            }
        }

        public static async Task<IEnumerable<Shield>> Load()
        {
#if !DEBUG
            if (await FileExist(ShieldFileName, StorageType.Local))
            {
                var file = await localFolder.GetFileAsync(ShieldFileName);
                using (var stream = await file.OpenStreamForReadAsync())
                {
                    var obj = (Shield[])serializer.Deserialize(stream);
                    stream.Flush();
                    return obj;
                }
            }

            if (await FileExist("ShieldsV3.xml", StorageType.Roaming))
                return await Upgrade("ShieldsV3.xml", StorageType.Roaming);

            if (await FileExist("ShieldsV2.xml", StorageType.Roaming))
                return await Upgrade("ShieldsV2.xml", StorageType.Roaming);

            if (await FileExist("Shields.xml", StorageType.Roaming))
                return await Upgrade("Shields.xml", StorageType.Roaming);

#endif
            return await GetNew();
        }


        private static async Task<bool> FileExist(string fileName, StorageType type)
        {
            try
            {
                if(type == StorageType.Local)
                    await localFolder.GetFileAsync(fileName);
                else
                    await roamingFolder.GetFileAsync(fileName);
                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
            catch (IOException)
            {
                return false;
                //throw
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


        private static async Task<IEnumerable<Shield>> Upgrade(string oldFile, StorageType type)
        {
            Shield[] oldShields;
            var file = type == StorageType.Local ?
                await localFolder.GetFileAsync(oldFile) :
                await roamingFolder.GetFileAsync(oldFile);

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