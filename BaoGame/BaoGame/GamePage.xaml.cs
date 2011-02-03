using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace BaoGame
{
    public partial class GamePage : PhoneApplicationPage
    {
        public GamePage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            int rowCount = 4;
            int colCount = 8;
          
            for (int i = 0; i < rowCount; i++)
            {
                BoardGrid.RowDefinitions.Add(new RowDefinition());
                BoardGrid.ColumnDefinitions.Add(new ColumnDefinition());

                for (int j = 0; j < colCount; j++)
                {
                    var b = new Border()
                    {
                        Background = new SolidColorBrush(Colors.Transparent),
                        BorderThickness = new Thickness(1),
                        Margin = new Thickness(3),
                        BorderBrush = new SolidColorBrush(Colors.White),
                        CornerRadius = new CornerRadius(5)
                    };

                    b.SetRow(i);
                    b.SetColumn(j);
                    b.MouseLeftButtonDown += Button_Click;
                    BoardGrid.Children.Add(b);
                }
            }
        }


    }
}