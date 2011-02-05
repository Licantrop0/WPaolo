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
using WPCommon;

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

                    b.Child = new Image()
                    {
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                        VerticalAlignment = System.Windows.VerticalAlignment.Center,
                        Source = new ImageSourceConverter().ConvertFromString("img/nyumba_85x85.png") as ImageSource

                       
                    };
                }
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var currentButton = (Border)sender;
            Point p = new Point(currentButton.GetColumn(), currentButton.GetRow());



        //    bool res = Square.PressButton(p);
        //    if (res && !cancel)
        //    {
        //        currentButton.Child = new TextBlock()
        //        {
        //            Text = Square.positionHistory.Count.ToString(),
        //            VerticalAlignment = System.Windows.VerticalAlignment.Center,
        //            TextAlignment = TextAlignment.Center,
        //            FontSize = 30
        //        };

        //        Completed.Stop();
        //        Storyboard.SetTarget(Completed, currentButton);
        //        Completed.Begin();

        //        if (Square.positionHistory.Count == (MagicGrid.RowDefinitions.Count * MagicGrid.ColumnDefinitions.Count))
        //        {
        //            dt.Stop();
        //            MessageBox.Show("Congratulations! Magic Square completed in " + sw.Elapsed.TotalSeconds + " seconds!");
        //            sw.Reset();
        //            end = true;
        //            Settings.RecordsList.Add(new Record(Square.ActualSize, DateTime.Now, sw.Elapsed));
        //        }
        //    }
        //    else if (res && cancel)
        //    {
        //        Completed.Stop();
        //        if (Square.positionHistory.Count > 0)
        //        {
        //            var lastButton = MagicGrid.Children
        //                .Where(b => b.GetRow() == Square.positionHistory.Peek().Y)
        //                .Where(b => b.GetColumn() == Square.positionHistory.Peek().X).Single();

        //            Storyboard.SetTarget(Completed, lastButton);
        //            Completed.Begin();
        //        }
        //    }
        //    else
        //    {
        //        //l'utente ha violato le regole quindi non si fa nulla, magari coloro il tasto di rosso per mezzo secondo?
        //        RedFlash.Stop();
        //        Storyboard.SetTarget(RedFlash, currentButton);
        //        RedFlash.Begin();
            
        }


    }
}