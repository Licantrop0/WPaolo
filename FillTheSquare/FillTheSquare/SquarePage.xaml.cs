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
using Microsoft.Phone.Controls;
using WPCommon;
using FillTheSquare.ViewModel;

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
			//ripristino dello stato della griglia
			var points = Square.PositionHistory.ToArray();
			Array.Reverse(points);
			for (int i = 0; i < points.Length; i++)
			{
				var b = MagicGrid.Children[Square.Size * points[i].Y + points[i].X] as Border;
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
						SoundManager.PlayOhNo();
						PhilPiangeAppear.Begin();
						NoMoreMovesTextBlock.Visibility = Visibility.Visible;
					}
					else
					{
						SoundManager.PlayMove();
					}
					break;

				case false: //caso di creazione fallito
					RedFlash.Stop();
					Storyboard.SetTarget(RedFlash, currentBorder);
					RedFlash.Begin();
					SoundManager.PlayError();
					break;

				case null: //caso di cancellazione
					ClearBorder(currentBorder);
					PhilPiangeDisappear.Begin();
					NoMoreMovesTextBlock.Visibility = Visibility.Collapsed;

					//Evidenzio la casella sull'ultimo premuto se la griglia non è vuota
					if (!Square.IsEmpty)
					{
						var lastValue = Square.PositionHistory.Peek();
						var lastButton = MagicGrid.Children
							.Where(b => b.GetRow() == lastValue.Y)
							.Where(b => b.GetColumn() == lastValue.X).First();

						SetFocus.Stop();
						Storyboard.SetTarget(SetFocus, lastButton);
						SetFocus.Begin();
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

		private static void ClearBorder(Border b)
		{
			b.Child = null;
			b.Background = (LinearGradientBrush)App.Current.Resources["BorderBackgroundBrush"];
		}

	}
}
