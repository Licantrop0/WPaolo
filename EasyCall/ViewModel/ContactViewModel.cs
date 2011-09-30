using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GalaSoft.MvvmLight;

namespace EasyCall
{
    public class ContactViewModel : ViewModelBase
    {
        public string DisplayName { get; set; }
        public string[] NumberRepresentation { get; set; }
        public string[] Numbers { get; set; }

        public ContactViewModel(string displayName, IEnumerable<string> numbers)
        {
            DisplayName = displayName;
            NumberRepresentation = TextToNum(displayName);
            Numbers = numbers.Select(n => Regex.Replace(n, @"[\s\-\(\)]", string.Empty)).ToArray();
        }

        private string[] TextToNum(string input)
        {
            if (string.IsNullOrEmpty(input))
                return new string[0];
            
            input = input.ToLower();
            var output = input.Split(' ');            

            for (var w = 0; w < output.Length; w++)
            {
                var word = output[w].ToCharArray();
                for (int i = 0; i < word.Length; i++)
                {
                    if (word[i] == '+') word[i] = '0';
                    else if (word[i] == 'a' || word[i] == 'b' || word[i] == 'c') word[i] = '2';
                    else if (word[i] == 'd' || word[i] == 'e' || word[i] == 'f') word[i] = '3';
                    else if (word[i] == 'g' || word[i] == 'h' || word[i] == 'i') word[i] = '4';
                    else if (word[i] == 'j' || word[i] == 'k' || word[i] == 'l') word[i] = '5';
                    else if (word[i] == 'm' || word[i] == 'n' || word[i] == 'o') word[i] = '6';
                    else if (word[i] == 'p' || word[i] == 'q' || word[i] == 'r' || word[i] == 's') word[i] = '7';
                    else if (word[i] == 't' || word[i] == 'u' || word[i] == 'v') word[i] = '8';
                    else if (word[i] == 'w' || word[i] == 'x' || word[i] == 'y' || word[i] == 'z') word[i] = '9';
                }

                output[w] = new string(word);
            }

            return output;
        }

    }
}
