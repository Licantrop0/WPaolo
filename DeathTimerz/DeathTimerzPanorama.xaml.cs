using System;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Controls;
using Microsoft.Xna.Framework.Media;
using DeathTimerz.Localization;

namespace DeathTimerz
{
    public partial class DeathTimerzPanorama : PhoneApplicationPage
    {
        private DispatcherTimer dt = new DispatcherTimer();

        public DeathTimerzPanorama()
        {
            InitializeComponent();
            InitializeTimer();
            InitializeApplicationBar();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (Settings.BirthDay.HasValue)
            {
                var DaysToBirthDay = ExtensionMethods.GetNextBirthday(Settings.BirthDay.Value).Subtract(DateTime.Now).Days;
                DaysToBirthdayTextBlock.Text = DaysToBirthDay.ToString() + " " +
                    (DaysToBirthDay == 1 ? AppResources.Day : AppResources.Days);
            }
            else
            {
                NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
                return;
            }

            dt_Tick(sender, EventArgs.Empty);

        }


        private void InitializeTimer()
        {
            dt.Interval = TimeSpan.FromMinutes(1);
            dt.Tick += dt_Tick;
            dt.Start();
        }


        void dt_Tick(object sender, EventArgs e)
        {
            if (!Settings.BirthDay.HasValue) return;

            //TODO: migliorare algoritmo
            var Age = DateTime.Now.Subtract(Settings.BirthDay.Value);

            var Years = Math.Floor(Age.TotalDays / Settings.AverageYear);
            var RemainingDays = Age.TotalDays - Years * Settings.AverageYear;
            var Months = Math.Floor(RemainingDays / Settings.AverageMonth);
            var Days = Math.Round(RemainingDays - Months * Settings.AverageMonth);

            YMDTextBlock.Text =
                Years.ToString("#0") + " " + (Years == 1 ? AppResources.Year : AppResources.Years) + "\n" +
                Months.ToString("0") + " " + (Months == 1 ? AppResources.Month : AppResources.Months) + "\n" +
                Days.ToString("0") + " " + (Days == 1 ? AppResources.Day : AppResources.Days) + "\n" +
                Age.Hours.ToString("0") + " " + (Age.Hours == 1 ? AppResources.Hour : AppResources.Hours) + "\n" +
                Age.Minutes.ToString("0") + " " + (Age.Minutes == 1 ? AppResources.Minute : AppResources.Minutes);// + "\n" +
            //Age.Seconds.ToString("0") + " " + (Age.Seconds == 1 ? AppResources.Second : AppResources.Seconds);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (Settings.EstimatedDeathAge.HasValue)
            {
                var EstimatedDeathDate = Settings.BirthDay.Value.Add(Settings.EstimatedDeathAge.Value);

                if (EstimatedDeathDate > DateTime.Now)
                {
                    var TotalDaysLived = (DateTime.Now - Settings.BirthDay.Value).TotalDays;
                    var TotalLifeDays = (EstimatedDeathDate - Settings.BirthDay.Value).TotalDays;
                    EstimatedDeathAge.Text = string.Format(AppResources.WillDie,
                        EstimatedDeathDate,
                        Settings.EstimatedDeathAge.Value.TotalDays / Settings.AverageYear,
                        TotalDaysLived / TotalLifeDays * 100,
                        TotalLifeDays - TotalDaysLived);
                }
                else
                    EstimatedDeathAge.Text = AppResources.YetAlive;

                TakeTestButton.Visibility = Visibility.Collapsed;
            }
            else
                TakeTestButton.Visibility = Visibility.Visible;

        }

        private void TakeTestButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/TestPage.xaml", UriKind.Relative));
        }


        private void MainPanorama_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            switch (MainPanorama.SelectedIndex)
            {
                case 2: //Death-Test
                    this.ApplicationBar.MenuItems.Clear();
                    ApplicationBar.MenuItems.Add(CreateDisclaimerAppBarMenuItem());
                    ApplicationBar.Buttons.Add(CreateEditTestAppBarButton());
                    break;
                default:
                    this.ApplicationBar.MenuItems.Clear();
                    ApplicationBar.MenuItems.Add(CreateAboutAppBarMenuItem());
                    if (ApplicationBar.Buttons.Count == 2)
                        ApplicationBar.Buttons.RemoveAt(1);
                    break;
            }
        }

        #region Application Bar

        private void InitializeApplicationBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Buttons.Add(CreateSettingsAppBarButton());
            ApplicationBar.MenuItems.Add(CreateAboutAppBarMenuItem());
        }

        private ApplicationBarIconButton CreateSettingsAppBarButton()
        {
            var SettingsAppBarButton = new ApplicationBarIconButton();
            SettingsAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_settings.png", UriKind.Relative);
            SettingsAppBarButton.Text = AppResources.Settings;
            SettingsAppBarButton.Click += delegate(object sender, EventArgs e)
            { NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative)); };
            return SettingsAppBarButton;
        }

        private ApplicationBarIconButton CreateEditTestAppBarButton()
        {
            var EditTestAppBarButton = new ApplicationBarIconButton();
            EditTestAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_edit.png", UriKind.Relative);
            EditTestAppBarButton.Text = AppResources.EditTest;
            EditTestAppBarButton.Click += delegate(object sender, EventArgs e)
            { NavigationService.Navigate(new Uri("/TestPage.xaml", UriKind.Relative)); };
            return EditTestAppBarButton;
        }

        private ApplicationBarMenuItem CreateDisclaimerAppBarMenuItem()
        {
            var DisclaimerAppBarMenuItem = new ApplicationBarMenuItem();
            DisclaimerAppBarMenuItem.Text = AppResources.Disclaimer;
            DisclaimerAppBarMenuItem.Click += delegate(object sender, EventArgs e)
            { NavigationService.Navigate(new Uri("/DisclaimerPage.xaml", UriKind.Relative)); };
            return DisclaimerAppBarMenuItem;
        }

        private ApplicationBarMenuItem CreateAboutAppBarMenuItem()
        {
            var AboutAppBarMenuItem = new ApplicationBarMenuItem();
            AboutAppBarMenuItem.Text = AppResources.About;
            AboutAppBarMenuItem.Click += delegate(object sender, EventArgs e)
            { NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative)); };
            return AboutAppBarMenuItem;
        }

        #endregion
    }

}
