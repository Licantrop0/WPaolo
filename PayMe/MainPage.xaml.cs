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

            if (!Settings.HourlyPayment.HasValue)
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
            CalculatePayment();
        }


        private void StartStopButton_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.CurrentStatus == Settings.Status.Stopped)
            {
                GoToStatus(Settings.Status.Started);
            }
            else
            {
                var sw = new Stopwatch(); sw.Start();
                if (MessageBox.Show(AppResources.AlertStopCounting, string.Empty, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    if (Settings.CurrentStatus == Settings.Status.Paused)
                        Settings.StartTime += sw.Elapsed; //CoE del tempo che ci si mette a premere OK nel messagebox
                    GoToStatus(Settings.Status.Stopped);
                }
                sw.Stop();
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
            Settings.CurrentStatus = status;
            switch (status)
            {
                case Settings.Status.Started:
                    Settings.StartTime = DateTime.Now;
                    Settings.PauseTimeSpan = new TimeSpan();
                    dt.Start();
                    break;
                case Settings.Status.Stopped:
                    Settings.PauseTimeSpan += DateTime.Now - Settings.StartPauseTime;
                    dt.Stop();
                    break;
                case Settings.Status.Paused:
                    Settings.StartPauseTime = DateTime.Now;
                    dt.Stop();
                    break;
                case Settings.Status.Resumed:
                    Settings.PauseTimeSpan += DateTime.Now - Settings.StartPauseTime;
                    dt.Start();
                    break;
                default:
                    break;
            }
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
                    VisualStateManager.GoToState(this, "Stopped", true);
                    break;
                case Settings.Status.Paused:
                    Settings.PauseTimeSpan += DateTime.Now - Settings.StartPauseTime;
                    VisualStateManager.GoToState(this, "Paused", true);
                    break;
                case Settings.Status.Resumed:
                    VisualStateManager.GoToState(this, "Started", true);
                    dt.Start();
                    break;
                default:
                    break;
            }
            CalculatePayment();
        }

        private void CalculatePayment()
        {
            var ElapsedTime = (DateTime.Now - Settings.StartTime) - Settings.PauseTimeSpan;

            //Il cuore dell'app è questo calcolo complicatissimo!!!
            //(TempoTrascorso - Resto della divisione con lo scatto).OreTotali * Prezzo Orario + Diritto di chiamata
                var Payment = FractionTimeSpan(ElapsedTime, Settings.Threshold).TotalHours
                * Settings.HourlyPayment + Settings.CallPay;

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
            var SettingsAppBarButton = new ApplicationBarIconButton();
            SettingsAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_settings.png", UriKind.Relative);
            SettingsAppBarButton.Text = AppResources.Settings;
            SettingsAppBarButton.Click += delegate(object sender, EventArgs e)
                { NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative)); };
            ApplicationBar.Buttons.Add(SettingsAppBarButton);
        }
    }
}