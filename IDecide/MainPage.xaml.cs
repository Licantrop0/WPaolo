using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WPCommon;

namespace IDecide
{
	public partial class MainPage : PhoneApplicationPage
	{
		Random rnd = new Random();
		object mutex = new object();
		public MainPage()
		{
			InitializeComponent();
			ApplicationBar = new ApplicationBar();
			CreateAppBar();

			var sd = new ShakeDetector();
			sd.ShakeDetected += (sender, e) =>
			{
				//Inserire un Mutex
				lock(mutex)
				{
					Dispatcher.BeginInvoke(() =>
					{ DecideButton_Click(sender, null); });
				}
			};
			sd.Start();

		}

		private void DecideButton_Click(object sender, RoutedEventArgs e)
		{
			RotateButton.Stop();
			var SelectedChoices = Settings.ChoicesGroup
				.Where(c => c.Key == Settings.SelectedGroup)
				.Select(c => c.Value)
				.ToList();

			AnswerTextBlock.Text = SelectedChoices.Count > 0 ?
				SelectedChoices[rnd.Next(SelectedChoices.Count)] :
				AppResources.NothingToDecide;
			RotateButton.Begin();
			//if (SelectedChoices.Count > 0)
			//    MessageBox.Show(SelectedChoices[rnd.Next(SelectedChoices.Count)]);
			//else
			//    MessageBox.Show(AppResources.NothingToDecide);
		}


		private void CreateAppBar()
		{
			var EditChoicesAppBarButton = new ApplicationBarIconButton();
			EditChoicesAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_settings.png", UriKind.Relative);
			EditChoicesAppBarButton.Text = AppResources.EditChoices;
			EditChoicesAppBarButton.Click += delegate(object sender, EventArgs e)
			{ NavigationService.Navigate(new Uri("/GroupChoicesPage.xaml", UriKind.Relative)); };
			ApplicationBar.Buttons.Add(EditChoicesAppBarButton);
		}
	}
}