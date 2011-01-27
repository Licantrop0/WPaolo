using System;
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
    }
}
