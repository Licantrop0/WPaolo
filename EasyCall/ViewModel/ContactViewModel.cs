using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using Windows.Storage;
using Windows.Storage.Streams;

namespace EasyCall.ViewModel
{
    [DataContract]
    public class ContactViewModel : IList, IEnumerable<NumberViewModel>
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string ImagePath { get; set; }

        public string[] NumberRepresentation { get; set; }
        public Uri ContactImage => ImagePath == null ? null : new Uri(ImagePath);

        [DataMember]
        public readonly IList<NumberViewModel> Numbers;

        public ContactViewModel()
        {
            //for serialization
        }

        public ContactViewModel(string name, IEnumerable<NumberViewModel> numbers, IRandomAccessStreamReference thumbnail)
        {
            Debug.WriteLine(name);
            Name = name;
            NumberRepresentation = TextToNum(name);
            Numbers = numbers.ToList();
            if (thumbnail != null)
                ImagePath = ((StorageFile)thumbnail).Path;
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

            return new string(output).Split(' ');
        }


        #region IList Implementation

        public bool IsFixedSize => true;

        public bool IsReadOnly => true;

        public int Count => Numbers?.Count ?? 0;

        public bool IsSynchronized => false;
        public object SyncRoot { get; }

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