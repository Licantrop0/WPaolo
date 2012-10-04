using Coding4Fun.Phone.Controls;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using Scudetti.Localization;
using Scudetti.Sound;
using Scudetti.ViewModel;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Scudetti.View
{
	public partial class ShieldsPage : PhoneApplicationPage
	{
		public ShieldsPage()
		{
			InitializeComponent();
		}

		private bool toastTapped;

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			//I calcoli successivi vanno fatti solo quando torno indietro da uno scudetto
			if (e.NavigationMode != NavigationMode.Back) return;

			//e ho già sbloccato degli scudetti
			if (AppContext.TotalShieldUnlocked == 0) return;

			var newLevelUnlocked = AppContext.TotalShieldUnlocked % AppContext.LockTreshold == 0;
			var newBonusLevelUnlocked = AppContext.TotalShieldUnlocked % AppContext.BonusTreshold == 0;

			//Se ho già mostrato il toast, non faccio niente
			if (AppContext.ToastDisplayed)
			{
				AppContext.ToastDisplayed = newLevelUnlocked || newBonusLevelUnlocked;
				return;
			}

			if (newLevelUnlocked)
			{
				int newLevelNumber = (AppContext.TotalShieldUnlocked / AppContext.LockTreshold) + 1;
				if (newLevelNumber <= 6)
				{
					var level = AppContext.Levels.Single(l => l.Number == newLevelNumber);
					ShowToast(level);
				}
			}
			else if (newBonusLevelUnlocked)
			{
				int newLevelNumber = ((AppContext.TotalShieldUnlocked / AppContext.BonusTreshold)) * 100;
				var level = AppContext.Levels.Single(l => l.Number == newLevelNumber);
				ShowToast(level);
			}
			else if (AppContext.GameCompleted) //Gioco completato!
			{
				SoundManager.PlayGoal();
				var toast = new ToastPrompt
				{
					Title = AppResources.GameFinishedTitle,
					Message = AppResources.GameFinished,
					TextWrapping = TextWrapping.Wrap,
					MillisecondsUntilHidden = 8000,
					ImageSource = new BitmapImage(new Uri("..\\Images\\soccer icon.png", UriKind.Relative))
				};
				toast.Show();
			}
		}

		private void ShowToast(LevelViewModel level)
		{
			SoundManager.PlayGoal();
			var toast = new ToastPrompt
			{
				Message = string.Format(AppResources.NewLevel, level.LevelName),
				ImageSource = new BitmapImage(new Uri("..\\Images\\soccer icon.png", UriKind.Relative))
			};
			toast.Tap += (s1, e1) =>
			{
				toastTapped = true;
				Messenger.Default.Send<LevelViewModel>(level);
			};
			toastTapped = false;
			toast.Show();
			AppContext.ToastDisplayed = true;
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			if (toastTapped) NavigationService.RemoveBackEntry();
		}
	}
}