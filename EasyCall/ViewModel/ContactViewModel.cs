using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
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
                return _contactImage;
            }
            set
            {
                _contactImage = value;
                RaisePropertyChanged("ContactImage");
            }
        }

        public ContactViewModel()
        {
            //for serialization
        }

        public ContactViewModel(string name, IEnumerable<NumberViewModel> numbers, IRandomAccessStreamReference thumbnail)
        {
            Name = name;
            NumberRepresentation = TextToNum(name);
            Numbers = numbers.ToList();
            if(ContactImage == null)
                LoadThumbnail(thumbnail);
        }

        private void LoadThumbnail(IRandomAccessStreamReference thumbnail)
        {
            if (thumbnail == null)
            {
                ContactImage = new Uri("Images/Contact.png", UriKind.Relative);
            }
            else if (thumbnail is StorageFile)
            {
                ContactImage = new Uri(((StorageFile)thumbnail).Path);
            }
            else //Need to write thumbnail to disk
            {
                var fileName = Name + Numbers.First().Number + ".png";
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
                    case 'à':
                    case 'á':
                    case 'â':
                    case 'ã':
                    case 'ä':
                    case 'å':
                    case 'b':
                    case 'c':
                    case 'ç':
                        output[i] = '2';
                        break;
                    case 'd':
                    case 'e':
                    case 'è':
                    case 'é':
                    case 'ê':
                    case 'ë':
                    case 'f':
                        output[i] = '3';
                        break;
                    case 'g':
                    case 'h':
                    case 'i':
                    case 'ì':
                    case 'í':
                    case 'î':
                    case 'ï':
                        output[i] = '4';
                        break;
                    case 'j':
                    case 'k':
                    case 'l':
                        output[i] = '5';
                        break;
                    case 'm':
                    case 'n':
                    case 'ñ':
                    case 'o':
                    case 'ò':
                    case 'ó':
                    case 'ô':
                    case 'õ':
                    case 'ö':
                    case 'ø':
                        output[i] = '6';
                        break;
                    case 'p':
                    case 'q':
                    case 'r':
                    case 's':
                    case 'š':
                        output[i] = '7';
                        break;
                    case 't':
                    case 'u':
                    case 'ù':
                    case 'ú':
                    case 'û':
                    case 'ü':
                    case 'v':
                        output[i] = '8';
                        break;
                    case 'w':
                    case 'x':
                    case 'y':
                    case 'ý':
                    case 'ÿ':
                    case 'z':
                    case 'ž':
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