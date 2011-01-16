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
            if (!Settings.HourlyPayment.HasValue)
                NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
            else
                SetStatus(Settings.CurrentStatus);
        }

        private void InitializeTimer()
        {
            dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromSeconds(1);
            dt.Tick += new EventHandler(dt_Tick);
        }

        void dt_Tick(object sender, EventArgs e)
        {
            if (Settings.CurrentStatus == Settings.Status.Paused)
                Settings.StartTime += dt.Interval;
            else
                CalculatePayment();
        }

        private void CalculatePayment()
        {
            var ElapsedTime = DateTime.Now - Settings.StartTime;
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

        private void StartStopButton_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.CurrentStatus == Settings.Status.Stopped)
            {
                SetStatus(Settings.Status.Started);
            }
            else
            {
                var sw = new Stopwatch(); sw.Start();
                if (MessageBox.Show(AppResources.AlertStopCounting, string.Empty, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    if (Settings.CurrentStatus == Settings.Status.Paused)
                        Settings.StartTime += sw.Elapsed; //CoE del tempo che ci si mette a premere OK nel messagebox
                    SetStatus(Settings.Status.Stopped);
                }
                sw.Stop();
            }
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.CurrentStatus == Settings.Status.Started ||
                Settings.CurrentStatus == Settings.Status.Resumed)
                SetStatus(Settings.Status.Paused);
            else if (Settings.CurrentStatus == Settings.Status.Paused)
                SetStatus(Settings.Status.Resumed);
        }

        private void SetStatus(Settings.Status status)
        {
            Settings.CurrentStatus = status;
            switch (status)
            {
                case Settings.Status.Started:
                    Settings.StartTime = DateTime.Now;
                    dt.Start();
                    VisualStateManager.GoToState(this, "Started", true);
                    break;
                case Settings.Status.Resumed:
                    dt.Start();
                    VisualStateManager.GoToState(this, "Started", true);
                    break;
                case Settings.Status.Paused:
                    dt.Start();
                    VisualStateManager.GoToState(this, "Paused", true);
                    break;
                case Settings.Status.Stopped:
                    Settings.StartTime = DateTime.Now;
                    dt.Stop();
                    VisualStateManager.GoToState(this, "Stopped", true);
                    break;
                default:
                    break;
            }
            CalculatePayment();
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