using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using Microsoft.Phone;

namespace EasyCall.Model
{
    public class Contact
    {
        public string DisplayName { get; private set; }
        private IEnumerable<string> _numberRepresentation;
        public IList<string> Numbers { get; private set; }
        public WriteableBitmap Bitmap  { get; private set; }

        public Contact(string displayName, IEnumerable<string> numbers, Stream imageStream)
        {
            DisplayName = displayName;
            _numberRepresentation = TextToNum(displayName);
            Numbers = numbers.Select(n => Regex.Replace(n, @"[\s\-\(\)]", string.Empty)).ToList();
            if (imageStream != null)
                Bitmap = PictureDecoder.DecodeJpeg(imageStream);
        }

        private List<string> TextToNum(string input)
        {
            if (string.IsNullOrEmpty(input))
                return new List<string>();

            var output = input.ToLower().ToCharArray();
            for (int i = 0; i < output.Length; i++)
            {
                if (output[i] == 'a' || output[i] == 'b' || output[i] == 'c') output[i] = '2';
                else if (output[i] == 'd' || output[i] == 'e' || output[i] == 'f') output[i] = '3';
                else if (output[i] == 'g' || output[i] == 'h' || output[i] == 'i') output[i] = '4';
                else if (output[i] == 'j' || output[i] == 'k' || output[i] == 'l') output[i] = '5';
                else if (output[i] == 'm' || output[i] == 'n' || output[i] == 'o') output[i] = '6';
                else if (output[i] == 'p' || output[i] == 'q' || output[i] == 'r' || output[i] == 's') output[i] = '7';
                else if (output[i] == 't' || output[i] == 'u' || output[i] == 'v') output[i] = '8';
                else if (output[i] == 'w' || output[i] == 'x' || output[i] == 'y' || output[i] == 'z') output[i] = '9';
            }

            var words = new string(output).Split(' ');
            var searchableNumbers = new List<string>();

            for (int i = 0; i < words.Length; i++)
            {
                searchableNumbers.Add(string.Join(" ", words, i, words.Length - i));
            }

            return searchableNumbers;
        }

        public bool ContainsNumber(string number)
        {
            return Numbers.Any(n => n.Contains(number));
        }

        public bool ContainsName(string number)
        {
            return _numberRepresentation.Any(nr => nr.StartsWith(number));
        }

        public string FirstNumber
        {
            get { return Numbers.First(); }
        }

        public string FullNumberRepresentation
        {
            get { return _numberRepresentation.First(); }
        }
    }
}
