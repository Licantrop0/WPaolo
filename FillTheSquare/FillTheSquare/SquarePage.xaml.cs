using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Phone.Controls;

namespace FillTheSquare
{
    public partial class SquarePage : PhoneApplicationPage
    {
        public MagicSquare Square;
        DispatcherTimer dt;
        Stopwatch sw;
        bool end;

        public SquarePage()
        {
            InitializeComponent();
            end = false;
            InitializeTimers();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            var size = int.Parse(NavigationContext.QueryString["size"]);
            Square = new MagicSquare(size);
            for (int i = 0; i < size; i++)
            {
                MagicGrid.RowDefinitions.Add(new RowDefinition());
                MagicGrid.ColumnDefinitions.Add(new ColumnDefinition());

                for (int j = 0; j < size; j++)
                {
                    var b = new Button();
                    b.SetRow(i);
                    b.SetColumn(j);
                    b.Click += Button_Click;
                    MagicGrid.Children.Add(b);
                }
            }
        }

        private void dt_Tick(object sender, EventArgs e)
        {
            TimeElapsedTextBlock.Text = "Seconds: " + sw.Elapsed.TotalSeconds.ToString("0");
        }

        private void InitializeTimers()
        {
            dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromSeconds(1);
            dt.Tick += new EventHandler(dt_Tick);
            dt.Start();
            sw = new Stopwatch();
            sw.Start();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (end) return;

            var currentButton = (Button)sender;
            Point p = new Point(currentButton.GetColumn(), currentButton.GetRow());
            bool cancel = false;
            if (p.Equals(Square.LastPosition))
            {
                currentButton.Content = "";
                cancel = true;
            }
            bool res = Square.PressButton(p);
            if (res && !cancel)
            {
                currentButton.Content = Square.LastValue.ToString();
                currentButton.Background = new SolidColorBrush(Colors.Green);
                if (Square.LastValue == (MagicGrid.RowDefinitions.Count * MagicGrid.ColumnDefinitions.Count))
                {
                    dt.Stop();
                    MessageBox.Show("Congratulations! Magic Square completed in " + sw.Elapsed.TotalSeconds + " seconds!");
                    sw.Reset();
                    end = true;
                    Settings.RecordsList.Add(new Record(Square.ActualSize, DateTime.Now, sw.Elapsed));
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

    }
}