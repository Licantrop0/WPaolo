using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using SortTheSquare.Localization;
using SortTheSquare.Sounds;
using WPCommon.Helpers;
using System.Linq;

namespace SortTheSquare
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
            CreateGrid();
            InitializeSquare();
        }

        private void CreateGrid()
        {
            for (int i = 0; i < Square.Size; i++)
            {
                MagicGrid.RowDefinitions.Add(new RowDefinition());
                MagicGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
        }

        private void InitializeSquare()
        {
            for (int i = 0; i < Square.Size; i++)
            {
                for (int j = 0; j < Square.Size; j++)
                {
                    if (Square.Grid[j, i] == 0)
                        continue; //salta la generazione del bordo

                    var b = new Border()
                    {
                        Background = (SolidColorBrush)Resources["PhoneAccentBrush"], //occhio al tema corrente!
                        BorderThickness = Settings.CurrentGridSize == 3 ? new Thickness(2) : new Thickness(1),
                        BorderBrush = (SolidColorBrush)Resources["PhoneBorderBrush"],
                        Child = new TextBlock()
                        {
                            Text = Square.Grid[j, i].ToString(),
                            Foreground = (SolidColorBrush)Resources["PhoneContrastForegroundBrush"],
                            //scala il font dinamicamente a seconda del numero di square
                            FontSize = -10 * Settings.CurrentGridSize + 90,
                            TextAlignment = TextAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center
                        }
                    };

                    b.SetRow(i);
                    b.SetColumn(j);
                    b.MouseLeftButtonDown += Button_Click;
                    MagicGrid.Children.Add(b);
                }
            }
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

            //TODO: farei partire il timer solo al primo click sulla griglia
            sw.Start();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var currentBorder = (Border)sender;
            var p = new GridPoint(currentBorder.GetColumn(), currentBorder.GetRow());
            var newPoint = Square.PressButton(p);
            if (newPoint.HasValue)
            {
                //Sposto la casella nella nuova posizione
                currentBorder.SetColumn(newPoint.Value.X);
                currentBorder.SetRow(newPoint.Value.Y);

                if (Square.IsCompleted()) //Vittoria!
                {
                    dt.Stop();
                    AdPlaceHolder.Children.Clear(); //fix per bug GoogleAd + NonLinearNavigationService

                    var r = new Record(Square.Size, DateTime.Now, sw.Elapsed);
                    Settings.Records.Add(r);
                    NavigationService.Navigate(new Uri("/CongratulationsPage.xaml?id=" + r.Id, UriKind.Relative));
                }
                else
                {
                    SoundManager.MoveSound.Play();
                    InitializeAd(); //TODO: trovare un altro modo più furbo (ad es. chiamare il metodo solo al primo click)
                }
            }
        }

        private void InitializeAd()
        {
            if (AdPlaceHolder.Children.Count == 1)
                return;

            //var GoogleAd = new Google.AdMob.Ads.WindowsPhone7.WPF.BannerAd() { AdUnitID = "a14da9d2bde5aea" };
            //AdPlaceHolder.Children.Add(GoogleAd);
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            SoundManager.ResetSound.Play();

            sw.Reset();
            sw.Start();

            MagicGrid.Children.Clear();
            Square = new MagicSquare(Settings.CurrentGridSize);
            InitializeSquare();
        }
    }
}
