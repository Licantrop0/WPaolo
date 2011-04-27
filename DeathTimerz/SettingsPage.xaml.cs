using System;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows;
using System.ComponentModel;
using DeathTimerz.Localization;
using Microsoft.Xna.Framework.Media;
using System.Windows.Media;

namespace DeathTimerz
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            BuildApplicationBar();
            if (Settings.BirthDay.HasValue)
            {
                BirthDayDatePicker.Value = Settings.BirthDay;
                BirthDayTimePicker.Value = Settings.BirthDay;
            }

            SoundOffImage.Visibility = Settings.MusicEnabled ? Visibility.Visible : Visibility.Collapsed;
            EnableDisableMusicTextBlock.Text = Settings.MusicEnabled ? AppResources.DisableMusic : AppResources.EnableMusic;
        }

        private void SaveAppBarButton_Click(object sender, EventArgs e)
        {
            if (BirthDayDatePicker.Value >= DateTime.Today)
            {
                MessageBox.Show(AppResources.ErrorFutureBirthday);
                return;
            }

            //Trick per evitare il bug del DatePicker quando si imposta 1600 come anno
            if (BirthDayDatePicker.Value < DateTime.Now.AddYears(-130))
            {
                MessageBox.Show(AppResources.ErrorTooOldBirthday);
                BirthDayDatePicker.Value = DateTime.Now.AddYears(-50);
                return;
            }

            var NewDate = new DateTime(
                   BirthDayDatePicker.Value.Value.Year,
                   BirthDayDatePicker.Value.Value.Month,
                   BirthDayDatePicker.Value.Value.Day,
                   BirthDayTimePicker.Value.Value.Hour,
                   BirthDayTimePicker.Value.Value.Minute,
                   BirthDayTimePicker.Value.Value.Second);

            Settings.BirthDay = NewDate;
            NavigationService.GoBack();
        }

        private void CancelAppBarButton_Click(object sender, EventArgs e)
        {
            if (Settings.BirthDay.HasValue)
                NavigationService.GoBack();
            else
                MessageBox.Show(AppResources.ErrorNoBirthay);
        }

        // Helper function to build a localized ApplicationBar
        private void BuildApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar
            ApplicationBar = new ApplicationBar();
            ApplicationBar.BackgroundColor = Color.FromArgb(255, 60, 60, 60);
            ApplicationBar.ForegroundColor = Colors.Red;
            // Create a new button and set the text value to the localized string from AppResources
            var SaveAppBarButton = new ApplicationBarIconButton();
            SaveAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_save.png", UriKind.Relative);
            SaveAppBarButton.Text = AppResources.Save;
            SaveAppBarButton.Click += new EventHandler(SaveAppBarButton_Click);

            var CancelAppBarButton = new ApplicationBarIconButton();
            CancelAppBarButton.IconUri = new Uri("Toolkit.Content\\ApplicationBar.Cancel.png", UriKind.Relative);
            CancelAppBarButton.Text = AppResources.Cancel;
            CancelAppBarButton.Click += new EventHandler(CancelAppBarButton_Click);
            ApplicationBar.Buttons.Add(CancelAppBarButton);

            ApplicationBar.Buttons.Add(SaveAppBarButton);
        }

        private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Settings.MusicEnabled = !Settings.MusicEnabled;

            if (Settings.MusicEnabled)
            {
                Settings.AskAndPlayMusic();
                SoundOffImage.Visibility = Visibility.Visible;
                EnableDisableMusicTextBlock.Text = AppResources.DisableMusic;
            }
            else
            {
                MediaPlayer.Stop();
                SoundOffImage.Visibility =  Visibility.Collapsed;
                EnableDisableMusicTextBlock.Text = AppResources.EnableMusic;
            }
        }
    }
}