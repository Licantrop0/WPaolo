using System;
using System.Collections.Generic;

namespace TouchColors.Helper
{
    public static class ExtensionMethods
    {
        public static T GetNextRandomItem<T>(this IList<T> items, T currentItem) where T : IEquatable<T>
        {
            T nextItem;
            var rnd = new Random();

            do
            {
                nextItem = items[rnd.Next(items.Count - 1)];
            } while (nextItem.Equals(currentItem));

            return nextItem;
        }
    }
}