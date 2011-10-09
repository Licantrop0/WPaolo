using System;
using System.Linq;
using System.Windows;
using IDecide.Localization;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace IDecide
{
	public partial class MainPage : PhoneApplicationPage
	{
		Random rnd = new Random();
		public MainPage()
		{
			InitializeComponent();
			CreateAppBar();

			//var sd = new ShakeDetector();
			//sd.ShakeDetected += (sender, e) =>
			//{
			//    Dispatcher.BeginInvoke(() =>
			//    { DecideButton_Click(sender, null); });
			//};
			//sd.Start();
		}

		private void DecideButton_Click(object sender, RoutedEventArgs e)
		{
			var selectedChoices = AppContext.Groups.Where(g => g.Model.IsSelected).Single().Model.Choices.ToList();
			AnswerTextBlock.Text = selectedChoices.Any() ?
				selectedChoices[rnd.Next(selectedChoices.Count)] :
				AppResources.NothingToDecide;
			RotateButton.Begin();
		}

		private void CreateAppBar()
		{
			var EditChoicesAppBarButton = new ApplicationBarIconButton();
			EditChoicesAppBarButton.Text = AppResources.EditChoices;
			EditChoicesAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_settings.png", UriKind.Relative);
			EditChoicesAppBarButton.Click += (sender, e) => {
				NavigationService.Navigate(new Uri("/View/ChoicesGroupPage.xaml", UriKind.Relative));
			};
			ApplicationBar.Buttons.Add(EditChoicesAppBarButton);

			var AboutAppBarMenuItem = new ApplicationBarMenuItem();
			AboutAppBarMenuItem.Text = AppResources.About;
			AboutAppBarMenuItem.Click += (sender, e) => {
				NavigationService.Navigate(new Uri("/View/AboutPage.xaml", UriKind.Relative)); };
			ApplicationBar.MenuItems.Add(AboutAppBarMenuItem);
		}
	}
}