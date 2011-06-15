using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using FillTheSquare.Localization;
using FillTheSquare.Model;
using FillTheSquare.Sounds;
using FillTheSquare.ViewModel;
using Microsoft.Phone.Controls;
using WPCommon;

namespace FillTheSquare
{
    public partial class SquarePage : PhoneApplicationPage
    {
        public MagicSquare Square;
        DispatcherTimer dt;
        StopwatchWrapper sw;

        public SquarePage()
        {
            InitializeComponent();
            InitializeTimers();
            InizializeSquare();
            SettingsViewModel.StopMusicIfControl();
        }

        private void InizializeSquare()
        {
            Square = new MagicSquare(Settings.CurrentGridSize, Settings.GetGridState());
            for (int i = 0; i < Settings.CurrentGridSize; i++)
            {
                MagicGrid.RowDefinitions.Add(new RowDefinition());
                MagicGrid.ColumnDefinitions.Add(new ColumnDefinition());

                for (int j = 0; j < Settings.CurrentGridSize; j++)
                {
                    var b = new Border()
                    {
                        Background = (LinearGradientBrush)App.Current.Resources["BorderBackgroundBrush"],
                        BorderThickness = Square.Size == 5 ? new Thickness(2) : new Thickness(1),
                        BorderBrush = new SolidColorBrush(Colors.White),
                    };

                    b.SetRow(i);
                    b.SetColumn(j);
                    b.MouseLeftButtonDown += Button_Click;
                    MagicGrid.Children.Add(b);
                }
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            //ripristino della griglia
            var points = Square.PositionHistory.ToArray();
            Array.Reverse(points);
            for (int i = 0; i < points.Length; i++)
            {
                var b = GetBorder(points[i].X, points[i].Y);
                b.Child = new TextBlock()
                {
                    Text = (i + 1).ToString(),
                    Style = (Style)Application.Current.Resources["SquareTitleStyle"],
                    FontSize = Square.Size == 5 ? 32 : 28
                };
            }

            //ripristino dello stato
            if (points.Length > 0)
                sw.Start();
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            //salvo lo stato della griglia e del timer
            Settings.SetGridState(Square.PositionHistory);
            Settings.CurrentElapsedTime = sw.Elapsed;
        }

        private void InitializeTimers()
        {
            dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromSeconds(0.1);
            dt.Tick += (sender, e) =>
            {
                TimeElapsedTextBlock.Text = string.Format(
                    AppResources.ElapsedTime, sw.Elapsed.TotalSeconds);
            };
            dt.Start();

            sw = new StopwatchWrapper(Settings.CurrentElapsedTime);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var currentBorder = (Border)sender;
            var p = new GridPoint(
                currentBorder.GetColumn(),
                currentBorder.GetRow());

            //tutta la logica della griglia è dentro il metodo PressButton
            switch (Square.PressButton(p))
            {
                case true: //caso creazione andato a buon fine
                    currentBorder.Child = new TextBlock()
                    {
                        Text = Square.PositionHistory.Count.ToString(),
                        Style = (Style)Application.Current.Resources["SquareTitleStyle"],
                        FontSize = 28
                    };

                    SetFocusAnimation(currentBorder);

                    if (Square.IsCompleted) //Vittoria!
                    {
                        dt.Stop();
                        var r = new Record(Square.Size, DateTime.Now, sw.Elapsed);
                        Settings.Records.Add(r);
                        NavigationService.Navigate(new Uri("/CongratulationsPage.xaml?id=" + r.Id, UriKind.Relative));
                        break;
                    }

                    var availableMoves = Square.GetAvailableMoves();

                    if (availableMoves.Count == 0)  //non ci sono più mosse disponibili
                    {
                        SoundManager.PlayOhNo();
                        PhilPiangeAppear.Begin();
                        NoMoreMovesTextBlock.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        SoundManager.PlayMove();

                        //Qua devo far venire verdi le caselle disponibili per la mossa successiva
                        GreenMeansAnimation(availableMoves.Select(move => GetBorder(move.X, move.Y)));
                    }
                    break;

                case false: //caso di creazione fallito
                    RedFlashAnimation(currentBorder);
                    SoundManager.PlayError();
                    break;

                case null: //caso di cancellazione
                    ClearBorder(currentBorder);

                    //Evidenzio la casella sull'ultimo premuto se la griglia non è vuota
                    if (!Square.IsEmpty)
                    {
                        var lastValue = Square.PositionHistory.Peek();
                        var lastBorder = GetBorder(lastValue.X, lastValue.Y);
                        SetFocusAnimation(lastBorder);
                        var borders = Square.GetAvailableMoves().Select(move => GetBorder(move.X, move.Y));
                        GreenMeansAnimation(borders);
                    }

                    SoundManager.PlayUndo();
                    break;
            }

            if (Square.PositionHistory.Count == 1)
                sw.Start();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            SoundManager.PlayReset();
            PhilPiangeDisappear.Begin();
            NoMoreMovesTextBlock.Visibility = Visibility.Collapsed;

            sw.Reset();
            Settings.CurrentElapsedTime = TimeSpan.Zero;
            Settings.SetGridState(new Stack<GridPoint>());

            Square.Clear();

            MagicGrid.Children
                .Where(ctrl => ctrl is Border)
                .Cast<Border>()
                .ForEach(b => ClearBorder(b));
        }


        #region Helpers

        private static void ClearBorder(Border b)
        {
            b.Child = null;
            b.Background = (LinearGradientBrush)App.Current.Resources["BorderBackgroundBrush"];
        }

        private Border GetBorder(int x, int y)
        {
            return MagicGrid.Children[Square.Size * y + x] as Border;
        }

        #endregion

        #region Animations

        public void RedFlashAnimation(Border border)
        {
            RedFlash.Stop();
            Storyboard.SetTarget(RedFlash, border);
            RedFlash.Begin();
        }

        public void SetFocusAnimation(Border border)
        {
            SetFocus.Stop();
            Storyboard.SetTarget(SetFocus, border);
            SetFocus.Begin();
        }

        public void GreenMeansAnimation(IEnumerable<Border> borders)
        {
            GreenMeansAvailable.Stop();
            GreenMeansAvailable.Children.Clear();

            foreach (var border in borders)
            {
                var ca1 = new ColorAnimation()
                {
                    Duration = new Duration(TimeSpan.FromSeconds(0.5)),
                    To = Colors.Green
                };
                Storyboard.SetTarget(ca1, border);
                Storyboard.SetTargetProperty(ca1,
                    new PropertyPath("(Border.Background).(GradientBrush.GradientStops)[0].(GradientStop.Color)"));
                GreenMeansAvailable.Children.Add(ca1);

                var ca2 = new ColorAnimation()
                {
                    Duration = new Duration(TimeSpan.FromSeconds(0.5)),
                    To = Colors.Green
                };
                Storyboard.SetTarget(ca2, border);
                Storyboard.SetTargetProperty(ca2,
                    new PropertyPath("(Border.Background).(GradientBrush.GradientStops)[1].(GradientStop.Color)"));
                GreenMeansAvailable.Children.Add(ca2);
            }

            GreenMeansAvailable.Begin();        
        }


        #endregion
    }
}
