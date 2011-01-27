﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace FillTheSquare
{
    public static class ExtensionHelper
    {
        public static int GetRow(this Button b)
        {
            return (int)b.GetValue(Grid.RowProperty);
        }

        public static int GetColumn(this Button b)
        {
            return (int)b.GetValue(Grid.ColumnProperty);
        }

        public static void SetRow(this Button b, int row)
        {
            b.SetValue(Grid.RowProperty, row);
        }

        public static void SetColumn(this Button b, int column)
        {
            b.SetValue(Grid.ColumnProperty, column);
        }
    }
}
