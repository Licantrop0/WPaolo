using System;
using System.Linq;
using System.Windows;
using IDecide.Localization;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Advertising.Mobile.UI;
using System.Threading;
using ShakeGestures;
using System.Windows.Controls;

namespace IDecide
{
    public partial class MainPage : PhoneApplicationPage
    {
        Random rnd = new Random();
        public MainPage()
        {
            InitializeComponent();
            CreateAppBar();

            ShakeGesturesHelper.Instance.ShakeGesture += (sender, e) =>
            {
                Dispatcher.BeginInvoke(() =>
                {
                    AppearCloudStoryboard.Begin();
                });
                Thread.Sleep(TimeSpan.FromSeconds(1.5));
            };

            ShakeGesturesHelper.Instance.MinimumRequiredMovesForShake = 4;
            ShakeGesturesHelper.Instance.Active = true;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (AdPlaceHolder.Children.Count == 1) //l'Ad c'è già
                return;

            AdPlaceHolder.Children.Add(new AdControl("572ef47c-fdda-4d58-ba1c-9cfd93c12d43", "10027370", true)
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

        private void DoubleAnimationUsingKeyFrames_Completed(object sender, EventArgs e)
        {
            var selectedChoices = AppContext.Groups.Where(g => g.Model.IsSelected).Single().Model.Choices.ToList();
            AnswerTextBlock.Text = selectedChoices.Any() ?
                selectedChoices[rnd.Next(selectedChoices.Count)] :
                AppResources.NothingToDecide;
        }
    }
}