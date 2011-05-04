using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using SortTheSquare.Localization;
using SortTheSquare.Sounds;
using WPCommon;

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

        /*
         * Generare ogni volta tutta la griglia causa
         * lievi problemi di performance sul device.
         * alla fine è solo uno swap: basta fare SetRow e SetColum
         * sui 2 button in modo appropriato.
         * Bisognerebbe creare una classe apposita che contiene
         * le informazioni dei bottoni da swappare ritornata
         * dal metodo PressButton
         */
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

            if (Square.PressButton(p))
            {
                //sono riuscito a spostare una casella, devo ridisegnare la griglia
                InvalidateSquare();

                if (Square.IsCompleted()) //Vittoria!
                {
                    dt.Stop();
                    AdPlaceHolder.Children.Clear(); //fix per bug GoogleAd
                    var r = new Record(Square.Size, DateTime.Now, sw.Elapsed);
                    Settings.Records.Add(r);
                    NavigationService.Navigate(new Uri("/CongratulationsPage.xaml?id=" + r.Id, UriKind.Relative));
                }
                else
                {
                    SoundManager.MoveSound.Play();
                    InitializeAd(); //TODO: trovare un altro modo più furbo
                }
            }
        }

        private void InitializeAd()
        {
            if (AdPlaceHolder.Children.Count == 1)
                return;

            var GoogleAd = new Google.AdMob.Ads.WindowsPhone7.WPF.BannerAd() { AdUnitID = "a14da9d2bde5aea" };
            AdPlaceHolder.Children.Add(GoogleAd);
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            SoundManager.ResetSound.Play();
            sw.Reset();
            sw.Start();
            Square = new MagicSquare(Settings.CurrentGridSize);
            InvalidateSquare();
        }
    }
}
