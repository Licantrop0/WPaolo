using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Windows.Storage;
using Windows.Storage.Streams;

namespace EasyCall.ViewModel
{
    public class ContactViewModel : List<NumberViewModel>
    {
        public string DisplayName { get; set; }
        public string[] NumberRepresentation { get; set; }
        public Uri ContactImage { get; private set; }

        public ContactViewModel(string displayName, IEnumerable<NumberViewModel> numbers, IRandomAccessStreamReference thumbnail)
            : base(numbers)
        {
            Debug.WriteLine(displayName);
            DisplayName = displayName;
            NumberRepresentation = TextToNum(displayName);
            if(thumbnail !=null)
                ContactImage = new Uri(((StorageFile)thumbnail).Path);
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
    }
}