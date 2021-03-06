﻿using System;
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
using Microsoft.Advertising.Mobile.UI;
using Microsoft.Phone.Controls;
using WPCommon.Helpers;

namespace FillTheSquare
{
    public partial class SquarePage : PhoneApplicationPage
    {
        MagicSquare Square;
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
            if (!AdPlaceHolder.Children.Any())
            {
                var adControl = new AdControl(
                    "75c2778e-42d6-4d37-a5d4-c7c159716e13",
                    "10022422", true) { Height = 80, Width = 480, };
                adControl.ErrorOccurred += adControl_ErrorOccurred;
                AdPlaceHolder.Children.Add(adControl);
            }
            if (Square.IsEmpty)
                return;

            //ripristino la griglia
            var points = Square.PositionHistory.ToArray();
            Array.Reverse(points);
            var b = new Border();
            for (int i = 0; i < points.Length; i++)
            {
                b = GetBorder(points[i]);
                b.Child = GetTextBlock(i + 1);
            }

            SetFocusAnimation(b);
            var lastAvailableMoves = Square.GetAvailableMoves();
            GreenMeansAnimation(lastAvailableMoves.Select(move => GetBorder(move)));

            if (lastAvailableMoves.Count == 0)
            {
                PhilPiangeAppear.Begin();
                NoMoreMovesTextBlock.Visibility = Visibility.Visible;
            }

            else if (points.Length > 0)
                sw.Start();
        }

        void adControl_ErrorOccurred(object sender, Microsoft.Advertising.AdErrorEventArgs e)
        {
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            AdPlaceHolder.Children.Clear();

            //salvo lo stato della griglia e del timer
            Settings.SetGridState(Square.PositionHistory);
            Settings.CurrentElapsedTime = sw.Elapsed;
            sw.Stop();
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
             //GAME OVER, can't do anything
            if (NoMoreMovesTextBlock.Visibility == Visibility.Visible)
                return;

            var currentBorder = (Border)sender;
            PressButton(currentBorder);
        }

        private void PressButton(Border currentBorder)
        {
            var result = Square.PressButton(GetPoint(currentBorder));
            var lastAvailableMoves = Square.GetAvailableMoves();

            //tutta la logica della griglia è dentro il metodo PressButton
            switch (result)
            {
                case true: //caso creazione andato a buon fine
                    currentBorder.Child = GetTextBlock(Square.PositionHistory.Count);
                    SetFocusAnimation(currentBorder);

                    if (Square.IsCompleted) //Vittoria! :)
                    {
                        dt.Stop();
                        var r = new Record(Square.Size, DateTime.Now, sw.Elapsed);
                        Settings.Records.Add(r);
                        NavigationService.Navigate(new Uri("/View/CongratulationsPage.xaml?id=" + r.Id, UriKind.Relative));
                        break;
                    }

                    GreenMeansAnimation(lastAvailableMoves.Select(move => GetBorder(move)));
                    if (lastAvailableMoves.Count == 0)  //Game Over! :(
                    {
                        SoundManager.PlayOhNo();
                        PhilPiangeAppear.Begin();
                        NoMoreMovesTextBlock.Visibility = Visibility.Visible;
                        sw.Stop();
                    }
                    else
                    {
                        SoundManager.PlayMove();
                    }
                    break;

                case false: //caso di creazione fallito
                    SoundManager.PlayError();
                    RedFlashAnimation(currentBorder);
                    break;

                case null: //caso di cancellazione
                    //prendo i border delle ultime mosse disponibili
                    var borders = lastAvailableMoves.Select(move => GetBorder(move));
                    ClearBorder(currentBorder);
                    GreenMeansAnimation(borders);

                    //Evidenzio la casella sull'ultimo premuto se la griglia non è vuota
                    if (!Square.IsEmpty)
                    {                        
                        var lastBorder = GetBorder(Square.PositionHistory.Peek());
                        SetFocusAnimation(lastBorder);
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

            foreach (Border b in MagicGrid.Children)
            {
                ClearBorder(b);
            }
        }


        #region Helpers

        private static void ClearBorder(Border b)
        {
            b.Child = null;
            b.Background = App.Current.Resources["BorderBackgroundBrush"] as LinearGradientBrush;
        }

        private Border GetBorder(GridPoint p)
        {
            return MagicGrid.Children[Square.Size * p.Y + p.X] as Border;
        }

        private GridPoint GetPoint(Border b)
        {
            return new GridPoint(b.GetColumn(), b.GetRow());
        }

        private TextBlock GetTextBlock(int i)
        {
            return new TextBlock()
            {
                Text = i.ToString(),
                Style = (Style)Application.Current.Resources["SquareTitleStyle"],
                FontSize = Square.Size == 5 ? 32 : 28
            };
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
            if (!SettingsViewModel.Instance.HintsEnabled)
                return;

            GreenMeansAvailable.Stop();
            GreenMeansAvailable.Children.Clear();

            foreach (var border in borders)
            {
                var ca1 = new ColorAnimation()
                {
                    Duration = new Duration(TimeSpan.FromSeconds(0.5)),
                    To = Color.FromArgb(255, 131, 255, 128)
                };
                Storyboard.SetTarget(ca1, border);
                Storyboard.SetTargetProperty(ca1,
                    new PropertyPath("(Border.Background).(GradientBrush.GradientStops)[0].(GradientStop.Color)"));
                GreenMeansAvailable.Children.Add(ca1);

                var ca2 = new ColorAnimation()
                {
                    Duration = new Duration(TimeSpan.FromSeconds(0.5)),
                    To = Color.FromArgb(178, 62, 153, 59)
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
