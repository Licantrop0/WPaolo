using System;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Diagnostics;

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
            if (Settings.IsTrialMode && Settings.AlreadyOpenedToday)
            {
                NavigationService.Navigate(new Uri("/TrialExpiredPage.xaml", UriKind.Relative));
                return;
            }

            if (!settings.HourlyPayment.HasValue)
                NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
            else
                ResumeStatus(Settings.CurrentStatus);
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
            if (Settings.CurrentStatus == Settings.Status.Stopped)
            {
                GoToStatus(Settings.Status.Started);
            }
            else
            {
                if (MessageBox.Show(AppResources.AlertStopCounting, string.Empty, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    GoToStatus(Settings.Status.Stopped);
                }
            }
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.CurrentStatus == Settings.Status.Started ||
                Settings.CurrentStatus == Settings.Status.Resumed)
                GoToStatus(Settings.Status.Paused);
            else if (Settings.CurrentStatus == Settings.Status.Paused)
                GoToStatus(Settings.Status.Resumed);
        }

        private void GoToStatus(Settings.Status status)
        {
            switch (status)
            {
                case Settings.Status.Started:
                    Settings.StartTime = DateTime.Now;
                    Settings.PauseTimeSpan = TimeSpan.Zero;
                    dt.Start();
                    VisualStateManager.GoToState(this, "Started", true);
                    break;
                case Settings.Status.Stopped:
                    if (Settings.CurrentStatus == Settings.Status.Paused)
                        Settings.PauseTimeSpan += DateTime.Now - Settings.StartPauseTime;
                    dt.Stop();
                    VisualStateManager.GoToState(this, "Stopped", true);
                    var a = new Attendance(Settings.StartTime, DateTime.Now, Settings.PauseTimeSpan,
                            CalculatePayment(), Settings.CurrencySymbol);
                    Settings.Attendances.Add(a);
                    NavigationService.Navigate(new Uri("/AddEditAttendance.xaml?id=" + a.Id, UriKind.Relative));
                    break;
                case Settings.Status.Paused:
                    Settings.StartPauseTime = DateTime.Now;
                    dt.Stop();
                    VisualStateManager.GoToState(this, "Paused", true);
                    break;
                case Settings.Status.Resumed:
                    Settings.PauseTimeSpan += DateTime.Now - Settings.StartPauseTime;
                    dt.Start();
                    VisualStateManager.GoToState(this, "Started", true);
                    break;
                default:
                    break;
            }
             Settings.CurrentStatus = status;
       }

        private void ResumeStatus(Settings.Status status)
        {
            switch (status)
            {
                case Settings.Status.Started:
                    VisualStateManager.GoToState(this, "Started", true);
                    dt.Start();
                    break;
                case Settings.Status.Stopped:
                    Settings.StartTime = DateTime.Now;
                    Settings.PauseTimeSpan = TimeSpan.Zero;
                    VisualStateManager.GoToState(this, "Stopped", true);
                    break;
                case Settings.Status.Paused:
                    Settings.PauseTimeSpan += DateTime.Now - Settings.StartPauseTime;
                    Settings.StartPauseTime = DateTime.Now;
                    VisualStateManager.GoToState(this, "Paused", true);
                    break;
                case Settings.Status.Resumed:
                    VisualStateManager.GoToState(this, "Started", true);
                    dt.Start();
                    break;
                default:
                    break;
            }
            DisplayPayment();
        }

        private void DisplayPayment()
        {
            var ElapsedTime = (DateTime.Now - Settings.StartTime) - Settings.PauseTimeSpan;

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


        private double CalculatePayment()
        {
            var ElapsedTime = (DateTime.Now - Settings.StartTime) - Settings.PauseTimeSpan;
            var i = FractionTimeSpan(ElapsedTime, settings.Threshold).TotalHours * settings.HourlyPayment.Value + settings.CallPay;
            return i;
        }


        private void CreateAppBar()
        {
            ApplicationBar = new ApplicationBar();
            var SettingsAppBarButton = new ApplicationBarIconButton();
            SettingsAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_settings.png", UriKind.Relative);
            SettingsAppBarButton.Text = AppResources.Settings;
            SettingsAppBarButton.Click += delegate(object sender, EventArgs e)
                { NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative)); };
            ApplicationBar.Buttons.Add(SettingsAppBarButton);
        }
    }
}