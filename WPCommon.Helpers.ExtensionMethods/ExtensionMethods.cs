using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace WPCommon.Helpers
{
    public static class ExtensionMethods
    {
        #region Grid Position Helper

        public static int GetRow(this UIElement uie)
        {
            return (int)uie.GetValue(Grid.RowProperty);
        }

        public static int GetColumn(this UIElement uie)
        {
            return (int)uie.GetValue(Grid.ColumnProperty);
        }

        public static void SetRow(this UIElement uie, int row)
        {
            uie.SetValue(Grid.RowProperty, row);
        }

        public static void SetColumn(this UIElement uie, int column)
        {
            uie.SetValue(Grid.ColumnProperty, column);
        }

        #endregion

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }

        public static void AddRange<T>(this ObservableCollection<T> source, IEnumerable<T> items)
        {
            foreach (var item in items)
                source.Add(item);
        }

//#if DEBUG
//        static Stopwatch sw;
//        public static void StartTrace(string message)
//        {
//            if (sw == null) sw = new Stopwatch();
//            sw.Start();
//            Debug.WriteLine(message);
//        }

//        public static void EndTrace()
//        {
//            if (sw == null) return;
//            sw.Stop();
//            Debug.WriteLine("Done in: " + sw.ElapsedMilliseconds + "ms");
//            sw.Reset();
//        }
//#endif
    }
}
