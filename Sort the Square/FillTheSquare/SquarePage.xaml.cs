using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using WPCommon;
using Google.AdMob.Ads.WindowsPhone7;
using Google.AdMob.Ads.WindowsPhone7.WPF;

namespace FillTheSquare
{
    public partial class SquarePage : PhoneApplicationPage
    {
        public MagicSquare Square;
        DispatcherTimer dt;
        Stopwatch sw;

        public SquarePage()
        {
            InitializeComponent();
            InitializeTimers();
            Square = new MagicSquare(Settings.CurrentGridSize);
            InitializeSquare();
        }

        private void InitializeSquare()
        {
            for (int i = 0; i < Settings.CurrentGridSize; i++)
            {
                MagicGrid.RowDefinitions.Add(new RowDefinition());
                MagicGrid.ColumnDefinitions.Add(new ColumnDefinition());
                for (int j = 0; j < Settings.CurrentGridSize; j++)
                {
                    var b = new Border()
                    {
                        Background = new SolidColorBrush(Colors.Cyan),
                        BorderThickness = Settings.CurrentGridSize == 5 ? new Thickness(2) : new Thickness(1),
                        BorderBrush = new SolidColorBrush(Colors.Black),
                        Child = new TextBlock()
                        {
                            Text = Square.Grid[j, i].ToString(),
                            Foreground = new SolidColorBrush(Colors.Green),
                            FontSize = 60,
                            TextAlignment = TextAlignment.Center,
                        }
                    };
                    if (Square.Grid[j, i] == 0)
                        b.Opacity = 0;
                    else
                        b.Opacity = 1;

                    b.SetRow(i);
                    b.SetColumn(j);
                    b.MouseLeftButtonDown += Button_Click;
                    MagicGrid.Children.Add(b);
                }
            }
        }

        private void InvalidateSquare()
        {
            MagicGrid.Children.Clear();
            MagicGrid.ColumnDefinitions.Clear();
            MagicGrid.RowDefinitions.Clear();
            InitializeSquare();
        }

        private void InitializeTimers()
        {
            dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromSeconds(1);
            dt.Tick += (sender, e) =>
                {
                    TimeElapsedTextBlock.Text = string.Format("{0}: {1:0}",
                        AppResources.Seconds, sw.Elapsed.TotalSeconds);
                };
            dt.Start();

            sw = new Stopwatch();
            sw.Start();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var currentBorder = (Border)sender;
            var p = new GridPoint(currentBorder.GetColumn(), currentBorder.GetRow());

            //tutta la logica della griglia è dentro il metodo PressButton
            switch (Square.PressButton(p))
            {
                case true: //sono riuscito a spostare una casella, devo ridisegnare la griglia
                    InvalidateSquare();

                    if (Square.IsCompleted()) //Vittoria!
                    {
                        dt.Stop();
                        var r = new Record(Square.Size, DateTime.Now, sw.Elapsed);
                        Settings.Records.Add(r);
                        NavigationService.Navigate(new Uri("/CongratulationsPage.xaml?id=" + r.Id, UriKind.Relative));
                        break;
                    }
                    else
                    {
                        Settings.MoveSound.Play();
                    }
                    break;

                case false: //non sono riuscito a spostare la casella, non faccio un Settings.ErrorSound.Play();
                    break;
            }
        }


        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.ResetSound.Play();
            sw.Reset();
            sw.Start();
            Square = new MagicSquare(Settings.CurrentGridSize);
            InvalidateSquare();
        }
    }
}
