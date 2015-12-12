using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using EasyCall.ViewModel;
using Windows.Storage;

namespace EasyCall.Helper
{
    public static class StorageHelper
    {
        private const string FileName = "contacts.db";

        public static async Task<List<ContactViewModel>> ReadAsync()
        {
            try { await ApplicationData.Current.LocalFolder.GetFileAsync(FileName); }
            catch (FileNotFoundException) { return null; }

            var serializer = new DataContractJsonSerializer(typeof(List<ContactViewModel>));
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(FileName))
            {
                return (List<ContactViewModel>)serializer.ReadObject(stream);
            }
        }

        public static async void WriteAsync(IEnumerable<ContactViewModel> data)
        {
            var serializer = new DataContractJsonSerializer(typeof(List<ContactViewModel>));
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(
                FileName, CreationCollisionOption.ReplaceExisting))
            {
                serializer.WriteObject(stream, data);
            }
        }

    }
}
