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
using System.Windows.Threading;
using Microsoft.Phone.Controls;

namespace FillTheSquare
{
    public partial class FiveSquarePage : PhoneApplicationPage
    {
        public MagicSquare Square;
        public TimeSpan TimeLeft;
        DispatcherTimer dt;
        private int seconds;
        bool end;
        public FiveSquarePage()
        {
            InitializeComponent();
            seconds = -1;
            end = false;
            InitializeTimer();
        }

        private void InitializeSquares()
        {
            var size = int.Parse(NavigationContext.QueryString["id"]);
            for (int i = 0; i < size; i++)
            {
                //if (i != 0)
                //{
                    MagicGrid.RowDefinitions.Add(new RowDefinition());
                    MagicGrid.ColumnDefinitions.Add(new ColumnDefinition());
                //}
                //else
                //{
                //    MagicGrid = new Grid();
                //    Thickness thik = new Thickness(12,0,12,0);
                //    MagicGrid.Margin = thik;
                //    MagicGrid.Height = 70;
                //    MagicGrid.Width = 70;
                //    MagicGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                //    MagicGrid.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                //}
                for (int j = 0; j < size; j++)
                {
                    var b = new Button();
                    b.SetValue(Grid.RowProperty, i);
                    b.SetValue(Grid.ColumnProperty, j);
                    b.Click += Button_Click;
                    MagicGrid.Children.Add(b);
                }
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            dt.Start();
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            dt.Stop();
        }

        private void dt_Tick(object sender, EventArgs e)
        {
            seconds++;
            secondsTextBlock.Text = seconds.ToString();
        }

        private void InitializeTimer()
        {
            dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromSeconds(1);
            dt.Tick += new EventHandler(dt_Tick);
            dt_Tick(null, EventArgs.Empty);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (end)
            {
                return;
            }
            var size = int.Parse(NavigationContext.QueryString["id"]);
            var currentButton = (Button)sender;
            Point p = new Point(currentButton.GetColumn(), currentButton.GetRow());
            bool cancel = false;
            if (p.Equals(Square.actualPosition))
            {
                currentButton.Content = "";
                cancel = true;
            }
            bool res = Square.PressButton(p);
            if (res && !cancel)
            {
                currentButton.Content = Square.actualValue.ToString();
                currentButton.Background = new SolidColorBrush(Colors.Green);
                if (Square.actualValue == (size*size))
                {
                    dt.Stop();
                    MessageBox.Show("Congratulations! Magic Square completed in " + seconds + " seconds!");
                    end = true;
                    string userName = null;
                    //TO DO: APRIRE MESSAGE BOX PER INSERIRE NOME
                    Settings.RecordsList.Add(new Record(seconds, userName, DateTime.Now));
                }
            }
            else if (res && cancel)
            {
                currentButton.Background = new SolidColorBrush(Colors.Transparent);
            }
            else
            {
                //l'utente ha violato le regole quindi non si fa nulla, magari coloro il tasto di rosso per mezzo secondo?
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            var size = int.Parse(NavigationContext.QueryString["id"]);
            if (size == 5)
                Square = new MagicSquare(SquareSize.Five);
            else
                Square = new MagicSquare(SquareSize.Ten);
            InitializeSquares();
        }
    }
}