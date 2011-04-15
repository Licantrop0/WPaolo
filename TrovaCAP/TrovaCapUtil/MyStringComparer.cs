using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrovaCapUtil
{
    class MyStringComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
        }
    }
}
