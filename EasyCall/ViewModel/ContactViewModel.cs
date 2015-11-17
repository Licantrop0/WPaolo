using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows;
using Windows.ApplicationModel.Contacts;
using Windows.Storage;
using Windows.Storage.Streams;
using WPCommon.Helpers;

namespace EasyCall.ViewModel
{
    [DataContract]
    public class ContactViewModel : ObservableObject, IList, IEnumerable<NumberViewModel>
    {
        private Uri _contactImage;

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public readonly IList<NumberViewModel> Numbers;

        [DataMember]
        public string[] NumberRepresentation { get; set; }

        [DataMember]
        public Uri ContactImage
        {
            get
            {
                Debug.WriteLine("ContactImage Get " + (_contactImage?.ToString() ?? "null"));
                return _contactImage;
            }
            set
            {
                Debug.WriteLine("ContactImage Set " + (value?.ToString() ?? "null"));
                _contactImage = value;
                RaisePropertyChanged("ContactImage");
            }
        }

        public ContactViewModel()
        {
            Debug.WriteLine("ContactViewModel ctor parameterless");
            //for serialization
        }

        public ContactViewModel(string name, IEnumerable<NumberViewModel> numbers, IRandomAccessStreamReference thumbnail)
        {
            Debug.WriteLine("ContactViewModel ctor with parameters");
            Name = name;
            NumberRepresentation = TextToNum(name);
            Numbers = numbers.ToList();
            if(ContactImage == null)
                LoadThumbnail(thumbnail);
        }

        private void LoadThumbnail(IRandomAccessStreamReference thumbnail)
        {
            Debug.WriteLine("LoadThumbnail " + (thumbnail == null ? "null" : "actual"));

            if (thumbnail == null)
            {
                ContactImage = new Uri("Images/Contact.png", UriKind.Relative);
            }
            else if (thumbnail is StorageFile)
            {
                ContactImage = new Uri(((StorageFile)thumbnail).Path);
            }
            else //Write file to disk
            {
                var fileName = Name + Numbers[0].Number + ".png";
                Debug.WriteLine("LoadThumbnail assigning _contactImage " + fileName);

                _contactImage = new Uri(Path.Combine(ApplicationData.Current.LocalFolder.Path, fileName));
                WriteThumbnail(fileName, thumbnail);
            }
        }

        private async void WriteThumbnail(string fileName, IRandomAccessStreamReference thumbnail)
        {
            var storageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(
                fileName,
                CreationCollisionOption.ReplaceExisting);

            using (var current = await storageFile.OpenStreamForWriteAsync())
            {
                var readstream = await thumbnail.OpenReadAsync();
                await readstream.AsStreamForRead().CopyToAsync(current);
            }

            Debug.WriteLine("WriteThumbnail writed to disk " + storageFile.Path);
            RaisePropertyChanged("ContactImage");
        }

        private static string[] TextToNum(string input)
        {
            if (string.IsNullOrEmpty(input))
                return new string[0];

            var output = input.ToLower().ToCharArray();
            for (var i = 0; i < output.Length; i++)
            {
                switch (output[i])
                {
                    case 'a':
                    case 'b':
                    case 'c':
                        output[i] = '2';
                        break;
                    case 'd':
                    case 'e':
                    case 'f':
                        output[i] = '3';
                        break;
                    case 'g':
                    case 'h':
                    case 'i':
                        output[i] = '4';
                        break;
                    case 'j':
                    case 'k':
                    case 'l':
                        output[i] = '5';
                        break;
                    case 'm':
                    case 'n':
                    case 'o':
                        output[i] = '6';
                        break;
                    case 'p':
                    case 'q':
                    case 'r':
                    case 's':
                        output[i] = '7';
                        break;
                    case 't':
                    case 'u':
                    case 'v':
                        output[i] = '8';
                        break;
                    case 'w':
                    case 'x':
                    case 'y':
                    case 'z':
                        output[i] = '9';
                        break;
                }
            }

            var s = new string(output);
            return s.Split(' '); //concat also the full string with space
        }

        #region IList Implementation

        public bool IsFixedSize => true;

        public bool IsReadOnly => true;

        public int Count => Numbers?.Count ?? 0;

        public bool IsSynchronized => false;
        public object SyncRoot => new object();

        public object this[int index]
        {
            get { return Numbers[index]; }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            return Numbers.Contains(value);
        }

        public int IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator() => Numbers.GetEnumerator();

        IEnumerator<NumberViewModel> IEnumerable<NumberViewModel>.GetEnumerator() => Numbers.GetEnumerator();

        #endregion
    }
}