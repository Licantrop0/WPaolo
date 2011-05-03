using System;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows;
using System.ComponentModel;
using DeathTimerz.Localization;
using Microsoft.Xna.Framework.Media;
using System.Windows.Media;
using DeathTimerz.Sounds;

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

            MusicToggleButton.DataContext = SoundManager.Instance;
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

        private void BuildApplicationBar()
        {
            var SaveAppBarButton = new ApplicationBarIconButton();
            SaveAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_save.png", UriKind.Relative);
            SaveAppBarButton.Text = AppResources.Save;
            SaveAppBarButton.Click += new EventHandler(SaveAppBarButton_Click);
            ApplicationBar.Buttons.Add(SaveAppBarButton);

            var CancelAppBarButton = new ApplicationBarIconButton();
            CancelAppBarButton.IconUri = new Uri("Toolkit.Content\\ApplicationBar.Cancel.png", UriKind.Relative);
            CancelAppBarButton.Text = AppResources.Cancel;
            CancelAppBarButton.Click += new EventHandler(CancelAppBarButton_Click);
            ApplicationBar.Buttons.Add(CancelAppBarButton);
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, CancelEventArgs e)
        {
            if (Settings.BirthDay == null)
                throw new Exception("ForceExit");
        }
    }
}