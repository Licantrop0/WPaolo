using System;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using WPCommon;

namespace PayMe
{
    public partial class MainPage : PhoneApplicationPage
    {
        DispatcherTimer dt;
        Settings settings = new Settings();

        public MainPage()
        {
            InitializeComponent();
            InitializeTimer();
            CreateAppBar();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (TrialManagement.IsTrialMode && TrialManagement.AlreadyOpenedToday)
            {
                NavigationService.Navigate(new Uri("/TrialExpiredPage.xaml", UriKind.Relative));
                return;
            }

            if (!settings.HourlyPayment.HasValue)
                NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
            else
                ResumeStatus(StatusManagement.CurrentStatus);
        }

        private void InitializeTimer()
        {
            dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromSeconds(1);
            dt.Tick += new EventHandler(dt_Tick);
        }

        void dt_Tick(object sender, EventArgs e)
        {
            DisplayPayment();
        }


        private void StartStopButton_Click(object sender, RoutedEventArgs e)
        {
            if (StatusManagement.CurrentStatus == Status.Stopped)
            {
                GoToStatus(Status.Started);
            }
            else
            {
                if (MessageBox.Show(AppResources.AlertStopCounting, string.Empty, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    GoToStatus(Status.Stopped);
                }
            }
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (StatusManagement.CurrentStatus == Status.Started ||
                StatusManagement.CurrentStatus == Status.Resumed)
                GoToStatus(Status.Paused);
            else if (StatusManagement.CurrentStatus == Status.Paused)
                GoToStatus(Status.Resumed);
        }

        private void GoToStatus(Status status)
        {
            switch (status)
            {
                case Status.Started:
                    StatusManagement.StartTime = DateTime.Now;
                    StatusManagement.PauseTimeSpan = TimeSpan.Zero;
                    dt.Start();
                    VisualStateManager.GoToState(this, "Started", true);
                    break;
                case Status.Stopped:
                    if (StatusManagement.CurrentStatus == Status.Paused)
                        StatusManagement.PauseTimeSpan += DateTime.Now - StatusManagement.StartPauseTime;

                    dt.Stop();
                    VisualStateManager.GoToState(this, "Stopped", true);
                    var a = new Attendance(StatusManagement.StartTime, DateTime.Now, StatusManagement.PauseTimeSpan,
                            CalculatePayment(), Settings.CurrencySymbol);
                    Settings.Attendances.Add(a);
                    NavigationService.Navigate(new Uri("/AddEditAttendance.xaml?id=" + a.Id, UriKind.Relative));
                    break;
                case Status.Paused:
                    StatusManagement.StartPauseTime = DateTime.Now;
                    dt.Stop();
                    VisualStateManager.GoToState(this, "Paused", true);
                    break;
                case Status.Resumed:
                    StatusManagement.PauseTimeSpan += DateTime.Now - StatusManagement.StartPauseTime;
                    dt.Start();
                    VisualStateManager.GoToState(this, "Started", true);
                    break;
                default:
                    break;
            }
            StatusManagement.CurrentStatus = status;
        }

        private void ResumeStatus(Status status)
        {
            switch (status)
            {
                case Status.Stopped:
                    StatusManagement.StartTime = DateTime.Now;
                    StatusManagement.PauseTimeSpan = TimeSpan.Zero;
                    VisualStateManager.GoToState(this, "Stopped", true);
                    break;
                case Status.Paused:
                    StatusManagement.PauseTimeSpan += DateTime.Now - StatusManagement.StartPauseTime;
                    StatusManagement.StartPauseTime = DateTime.Now;
                    VisualStateManager.GoToState(this, "Paused", true);
                    break;
                case Status.Started:
                case Status.Resumed:
                    VisualStateManager.GoToState(this, "Started", true);
                    dt.Start();
                    break;
                default:
                    break;
            }
            DisplayPayment();
        }

        private double CalculatePayment()
        {
            var ElapsedTime = (DateTime.Now - StatusManagement.StartTime) - StatusManagement.PauseTimeSpan;
            return FractionTimeSpan(ElapsedTime, settings.Threshold).TotalHours * settings.HourlyPayment.Value + settings.CallPay;
        }

        private void DisplayPayment()
        {
            var ElapsedTime = (DateTime.Now - StatusManagement.StartTime) - StatusManagement.PauseTimeSpan;

            //Il cuore dell'app è questo calcolo complicatissimo!!!
            //(TempoTrascorso - Resto della divisione con lo scatto).OreTotali * Prezzo Orario + Diritto di chiamata
            var Payment = FractionTimeSpan(ElapsedTime, settings.Threshold).TotalHours * settings.HourlyPayment.Value + settings.CallPay;

            TotalTextBlock.Text = string.Format("{0:0.00} {1}", Payment, Settings.CurrencySymbol);

            ElapsedTimeTextBlock.Text = string.Format("{0:00}:{1:00}:{2:00}",
                ElapsedTime.TotalHours, ElapsedTime.Minutes, ElapsedTime.Seconds);
        }

        private TimeSpan FractionTimeSpan(TimeSpan t1, TimeSpan t2)
        {
            return TimeSpan.FromTicks(t1.Ticks - t1.Ticks % t2.Ticks);
        }

        private void CreateAppBar()
        {
            ApplicationBar = new ApplicationBar();

            var AttendanceListAppBarButton = new ApplicationBarIconButton();
            AttendanceListAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar.list.png", UriKind.Relative);
            AttendanceListAppBarButton.Text = AppResources.AttendancesList;
            AttendanceListAppBarButton.Click += delegate(object sender, EventArgs e)
            { NavigationService.Navigate(new Uri("/AttendancesListPage.xaml", UriKind.Relative)); };
            ApplicationBar.Buttons.Add(AttendanceListAppBarButton);

            var SettingsAppBarButton = new ApplicationBarIconButton();
            SettingsAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_settings.png", UriKind.Relative);
            SettingsAppBarButton.Text = AppResources.Settings;
            SettingsAppBarButton.Click += delegate(object sender, EventArgs e)
                { NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative)); };
            ApplicationBar.Buttons.Add(SettingsAppBarButton);

            var AboutAppBarMenuItem = new ApplicationBarMenuItem();
            AboutAppBarMenuItem.Text = AppResources.About;
            AboutAppBarMenuItem.Click += delegate(object sender, EventArgs e)
            { NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative)); };
            ApplicationBar.MenuItems.Add(AboutAppBarMenuItem);
        }
    }
}