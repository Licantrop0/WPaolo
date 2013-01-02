using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Audio;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPCommon.Helpers;
using TwentyTwelve_Organizer.Sound;

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
            if (TrialManagement.IsTrialMode)
                DemoTextBlock.Visibility = System.Windows.Visibility.Visible;
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
            var CompletedTasksCount = 10;// AppContext.Tasks.Where(t => t.IsCompleted).Count(); 
            TasksProgressBar.Maximum = AppContext.Tasks.Count;            
            TasksProgressBar.Value = CompletedTasksCount;
            ProgressTextBlock.Text = string.Format("Task Completed: {0}/{1}", CompletedTasksCount, AppContext.Tasks.Count);
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            dt.Stop();
        }

        private void ViewTaskButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/TasksPage.xaml", UriKind.Relative));
        }

        void dt_Tick(object sender, EventArgs e)
        {
            TimeLeft = AppContext.EndOfTheWorld - DateTime.Now;
            t_countdowndays.Text = TimeLeft.Days.ToString();
            t_countdownhours.Text = TimeLeft.Hours.ToString();
            t_countdownminutes.Text = TimeLeft.Minutes.ToString();
            t_countdownseconds.Text = TimeLeft.Seconds.ToString();
            SoundManager.TickSound.Play();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //Calcolo per la valutazione (somma delle difficoltà dei task completati)
            //I valori delle difficoltà sono impostati nella classe Task
            int giorniNecessari = 20;//AppContext.Tasks.Where(t => !t.IsCompleted).Sum(t => (int)t.Difficulty);
            if (TimeLeft.TotalDays < 0)
            {
                EvalTextBlock.Text = "Open your eyes. Everything is different. Have a nice life.";
            }
            else if (TimeLeft.TotalDays == 0)
            {
                EvalTextBlock.Text = "THE END IS TODAY! Close your eyes and accept it!";
            }
            else if (AppContext.Tasks.Count == 0)
            {
                EvalTextBlock.Text = "You have no tasks.\nYou should spend your last days on earth with something to do!";
            }
            else
            {
                var rapportoGiorni = giorniNecessari / TimeLeft.TotalDays;

                if (rapportoGiorni > 1)
                {
                    EvalTextBlock.Text = "Do something or you will never complete all your tasks! Change your priorities or engage actively to fulfill your difficult endeavours...";
                }
                else if (rapportoGiorni <= 1 && rapportoGiorni > 0.75)
                {
                    EvalTextBlock.Text = "You must work harder to complete your tasks list.";
                }
                else if (rapportoGiorni <= 0.75 && rapportoGiorni > 0.50)
                {
                    EvalTextBlock.Text = "You are doing fine with your tasks, hold on!";
                }
                else if (rapportoGiorni <= 0.50 && rapportoGiorni > 0.25)
                {
                    EvalTextBlock.Text = "Well done, you can do it with a bit more of work.";
                }
                else
                {
                    EvalTextBlock.Text = "Greetings my friend, your committment to your tasks list is admirable!";
                }
            }

        }


        private void Button_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            SoundManager.ButtonDownSound.Play();
        }

        private void Button_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            SoundManager.ButtonUpSound.Play();
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/AboutPage.xaml", UriKind.Relative));
        }
    }
}
