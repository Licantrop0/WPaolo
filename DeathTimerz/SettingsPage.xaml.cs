using System;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows;
using System.ComponentModel;
using DeathTimerz.Localization;

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
        }

        private void SaveAppBarButton_Click(object sender, EventArgs e)
        {
            var NewDate = new DateTime(
                BirthDayDatePicker.Value.Value.Year,
                BirthDayDatePicker.Value.Value.Month,
                BirthDayDatePicker.Value.Value.Day,
                BirthDayTimePicker.Value.Value.Hour,
                BirthDayTimePicker.Value.Value.Minute,
                BirthDayTimePicker.Value.Value.Second);

            if (BirthDayDatePicker.Value >= DateTime.Today)
            {
                MessageBox.Show(AppResources.ErrorFutureBirthday);
            }
            //Trick per evitare il bug del DatePicker quando si imposta 1600 come anno
            else if (BirthDayDatePicker.Value < new DateTime(1700, 1, 1))
            {
                MessageBox.Show(AppResources.ErrorTooOldBirthday);
                BirthDayDatePicker.Value = new DateTime(1700, 1, 1);
            }
            else
            {
                Settings.BirthDay = NewDate;
                NavigationService.GoBack();
            }
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

            // Create a new button and set the text value to the localized string from AppResources
            var SaveAppBarButton = new ApplicationBarIconButton();
            SaveAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_save.png", UriKind.Relative);
            SaveAppBarButton.Text = AppResources.Save;
            SaveAppBarButton.Click += new EventHandler(SaveAppBarButton_Click);

            var CancelAppBarButton = new ApplicationBarIconButton();
            CancelAppBarButton.IconUri = new Uri("Toolkit.Content\\ApplicationBar.Cancel.png", UriKind.Relative);
            CancelAppBarButton.Text = AppResources.Cancel;
            CancelAppBarButton.Click += new EventHandler(CancelAppBarButton_Click);

            ApplicationBar.Buttons.Add(SaveAppBarButton);
            ApplicationBar.Buttons.Add(CancelAppBarButton);
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, CancelEventArgs e)
        {
            if(!Settings.BirthDay.HasValue)
                throw new Exception("ForceExit");
        }
    }
}