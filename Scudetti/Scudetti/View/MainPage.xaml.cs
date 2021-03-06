﻿using System;
using Microsoft.Advertising.Mobile.UI;
using Microsoft.Phone.Controls;
using Scudetti.Sound;
using System.Linq;
using System.Windows.Controls;

namespace Scudetti.View
{
	public partial class MainPage : PhoneApplicationPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
		{
			//se tutti gli scudetti sono completati, mostra lo scudetto al posto del pallone
			if (AppContext.GameCompleted)
				PlayButton.Template = (ControlTemplate)this.Resources["WinButtonTemplate"];

			if (AdPlaceHolder.Children.Count == 1) //l'Ad c'è già
				return;

			AdPlaceHolder.Children.Add(new AdControl("489510bc-7ee7-4d2d-9bf1-9065ff63354d", "10040107", true)
			{
				Height = 80,
				Width = 480,
			});

			base.OnNavigatedTo(e);
		}

		protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
		{
			AdPlaceHolder.Children.Clear();
			base.OnNavigatedFrom(e);
		}

		private void PlayButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			SoundManager.PlayKick();
			NavigationService.Navigate(new Uri("/View/LevelsPage.xaml", UriKind.Relative));
		}

		private void SettingsButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			SoundManager.PlayFischietto();
			NavigationService.Navigate(new Uri("/View/SettingsPage.xaml", UriKind.Relative));
		}

		private void RulesButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			SoundManager.PlayFischietto();
			NavigationService.Navigate(new Uri("/View/RulesPage.xaml", UriKind.Relative));
		}
	}
}