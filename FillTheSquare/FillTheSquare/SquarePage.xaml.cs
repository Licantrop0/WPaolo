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
using Microsoft.Xna.Framework.Audio;
using System.ComponentModel;

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
                    var b = new Border()
                    {
                        Background = (LinearGradientBrush)App.Current.Resources["BorderBackgroundBrush"],
                        BorderThickness = size == 5 ? new Thickness(2) : new Thickness(1),
                        BorderBrush = new SolidColorBrush(Colors.White),
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
            sw.Start();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //La trial scade dopo 99 secondi
            if (WPCommon.TrialManagement.IsTrialMode && sw.Elapsed.TotalSeconds >= 99)
            {
                NavigationService.Navigate(new Uri("/DemoPage.xaml", UriKind.Relative));
                return;
            }

            var currentBorder = (Border)sender;
            var p = new GridPoint(currentBorder.GetColumn(), currentBorder.GetRow());

            //tutta la logica della griglia è dentro il metodo PressButton
            switch (Square.PressButton(p))
            {
                case true: //caso creazione andato a buon fine
                    currentBorder.Child = new TextBlock()
                    {
                        Text = Square.positionHistory.Count.ToString(),
                        Style = (Style)Application.Current.Resources["SquareTitleStyle"],
                        FontSize = 28
                    };

                    SetFocus.Stop();
                    Storyboard.SetTarget(SetFocus, currentBorder);
                    SetFocus.Begin();

                    if (Square.IsCompleted) //Vittoria!
                    {
                        dt.Stop();
                        var r = new Record(Square.Size, DateTime.Now, sw.Elapsed);
                        Settings.Records.Add(r);
                        NavigationService.Navigate(new Uri("/CongratulationsPage.xaml?id=" + r.Id, UriKind.Relative));
                        break;
                    }

                    if (Square.GetMovesLeft() == 0)  //non ci sono più mosse disponibili
                    {
                        Settings.OhNoSound.Play();
                        PhilPiangeAppear.Begin();
                    }
                    else
                    {
                        Settings.MoveSound.Play();
                    }
                    break;

                case false: //caso di creazione fallito
                    RedFlash.Stop();
                    Storyboard.SetTarget(RedFlash, currentBorder);
                    RedFlash.Begin();
                    Settings.ErrorSound.Play();
                    break;

                case null: //caso di cancellazione
                    ClearBorder(currentBorder);
                    PhilPiangeDisappear.Begin();

                    //Evidenzio la casella sull'ultimo premuto se la griglia non è vuota
                    if (!Square.IsEmpty)
                    {
                        var lastValue = Square.positionHistory.Peek();
                        var lastButton = MagicGrid.Children
                            .Where(b => b.GetRow() == lastValue.Y)
                            .Where(b => b.GetColumn() == lastValue.X).First();

                        SetFocus.Stop();
                        Storyboard.SetTarget(SetFocus, lastButton);
                        SetFocus.Begin();
                    }
                    Settings.UndoSound.Play();
                    break;
            }
        }

        private void BackgroundMediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            var me = (MediaElement)sender;
            me.Stop();
            me.Play();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.ResetSound.Play();
            PhilPiangeDisappear.Begin();

            sw.Reset();
            sw.Start();

            Square.Clear();
            foreach (Border b in MagicGrid.Children
                .Where(ctrl => ctrl is Border))
                ClearBorder(b);

        }

        private static void ClearBorder(Border b)
        {
            b.Child = null;
            b.Background = (LinearGradientBrush)App.Current.Resources["BorderBackgroundBrush"];
        }

    }
}
