using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Audio;
using System.Windows.Input;

namespace TwentyTwelve_Organizer
{
    public partial class TimerPage : PhoneApplicationPage
    {
        TimeSpan TimeLeft;
        DispatcherTimer dt;

        public TimerPage()
        {
            InitializeComponent();
            InitializeTimer();
            if (Settings.IsTrialMode)
            {
                DemoTextBlock.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void InitializeTimer()
        {
            dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromSeconds(1);
            dt.Tick += new EventHandler(dt_Tick);
            dt_Tick(null, EventArgs.Empty);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            dt.Start();
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            dt.Stop();
        }

        private void ViewTaskButton_Click(object sender, RoutedEventArgs e)
        {
                NavigationService.Navigate(new Uri("/TasksPage.xaml", UriKind.Relative));
        }

        void dt_Tick(object sender, EventArgs e)
        {
            TimeLeft = Settings.EndOfTheWorld - DateTime.Now;
            t_countdowndays.Text = TimeLeft.Days.ToString();
            t_countdownhours.Text = TimeLeft.Hours.ToString();
            t_countdownminutes.Text = TimeLeft.Minutes.ToString();
            t_countdownseconds.Text = TimeLeft.Seconds.ToString();
            Settings.TickSound.Play();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //Calcolo per la valutazione (somma delle difficoltà dei task completati)
            //I valori delle difficoltà sono impostati nella classe Task
            int giorniNecessari = Settings.Tasks.Where(t => !t.IsCompleted).Sum(t => (int)t.Difficulty);
            if (TimeLeft.Days == 0)
            {
                EvalTextBlock.Text = "THE END IS TODAY! Close your eyes and accept it!";
            }
            else if (TimeLeft.Days < 0)
            {
                EvalTextBlock.Text = "Open your eyes. Everything is different. Have a nice life.";
            }
            else
            {
                var differenzaGiorni = TimeLeft.TotalDays - giorniNecessari;
                if (differenzaGiorni < 0)
                {
                    EvalTextBlock.Text = "You MUST change something in your tasks organization or you will not be prepared for the end!";
                }
                else if (differenzaGiorni >= 0 && differenzaGiorni < 15)
                {
                    EvalTextBlock.Text = "You have to work in a different way with your tasks, there is not so much time...";
                }
                else if (differenzaGiorni >= 15 && differenzaGiorni < 40)
                {
                    EvalTextBlock.Text = "You are doing well with your tasks.";
                }
                else if (differenzaGiorni >= 40 && differenzaGiorni < 90)
                {
                    EvalTextBlock.Text = "Very good, your tasks organization is brilliant, you will be ready for the end.";
                }
                else
                {
                    EvalTextBlock.Text = "You have worked so well.\nYou can take a rest, you have nothing to worry about the end.";
                }
            }

        }


        private void ViewTaskButton_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            Settings.ButtonDownSound.Play();
        }

        private void ViewTaskButton_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            Settings.ButtonUpSound.Play();
        }

    }
}