using System;
using System.Collections.Generic;

namespace Scudetti.Helper
{
    public static class ExtensionMethods
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            var rnd = new Random();

            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
