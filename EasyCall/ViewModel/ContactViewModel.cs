using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace EasyCall
{
    public class ContactViewModel
    {
        public string DisplayName { get; set; }
        public string NumberRepresentation { get; set; }
        public IEnumerable<string> Numbers { get; set; }

        public ContactViewModel(string displayName, IEnumerable<string> numbers)
        {
            DisplayName = displayName;
            NumberRepresentation = TextToNum(displayName);
            Numbers = numbers.Select(n => Regex.Replace(n,
                @"^(00|\+)(1|2[078]|2[1234569][0-9]|3[0123469]|3[578][0-9]|4[013456789]|42[0-9]|5[09][0-9]|5[12345678]|6[0123456]|6[789][0-9]|7|8[0578][0-9]|8[123469]|9[0123458]|9[679][0-9])", "0")
                .Replace(" ", ""));
        }

        private string TextToNum(string input)
        {
            if (input == null) return string.Empty;
            string output = string.Empty;
            foreach (char c in input)
            {
                if (c == '+') output += 0;
                if (c == ' ') output += 1;
                if (c == 'a' || c == 'b' || c == 'c') output += 2;
                if (c == 'd' || c == 'e' || c == 'f') output += 3;
                if (c == 'g' || c == 'h' || c == 'i') output += 4;
                if (c == 'j' || c == 'k' || c == 'l') output += 5;
                if (c == 'm' || c == 'n' || c == 'o') output += 6;
                if (c == 'p' || c == 'q' || c == 'r' || c == 's') output += 7;
                if (c == 't' || c == 'u' || c == 'v') output += 8;
                if (c == 'w' || c == 'x' || c == 'y' || c == 'z') output += 9;
            }
            return output;
        }
    }
}
