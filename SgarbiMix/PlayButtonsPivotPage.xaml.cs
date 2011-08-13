using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using SgarbiMix.ViewModel;
using WPCommon;
using WPCommon.Helpers;

namespace SgarbiMix
{
    public partial class PlayButtonsPivotPage : PhoneApplicationPage
    {
        private int counter;

        PlayButtonsViewModel _vm;
        public PlayButtonsViewModel VM
        {
            get
            {
                if (_vm == null)
                    _vm = LayoutRoot.DataContext as PlayButtonsViewModel;
                return _vm;
            }
        }

        public PlayButtonsPivotPage()
        {
            InitializeComponent();
            InizializeShaker();
        }

        private void InizializeShaker()
        {
            var _bCanExecuteSound = true;
            var sd = new ShakeDetector();
            var tmr = new DispatcherTimer();
            var rnd = new Random();

            tmr.Interval = TimeSpan.FromMilliseconds(700);
            tmr.Tick += (sender, e) =>
            {
                tmr.Stop();
                _bCanExecuteSound = true;
            };

            sd.ShakeDetected += (sender, e) =>
            {
                if (!CheckTrial()) return;

                Dispatcher.BeginInvoke(() =>
                {
                    if (_bCanExecuteSound)
                        VM.SoundResources[rnd.Next(VM.SoundResources.Length)].Play();

                    _bCanExecuteSound = false;
                    tmr.Start();
                });
            };

            sd.Start();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckTrial()) return;
            var b = sender as Button;
            var sound = b.DataContext as Model.SoundContainer;
            sound.Play();
        }


        private bool CheckTrial()
        {
            if (TrialManagement.IsTrialMode && (App.alreadyOpen || counter > 5))
            {
                NavigationService.Navigate(new Uri("/DemoPage.xaml", UriKind.Relative));
                return false;
            }
            //incremento ogni volta che chiamo checkTrial - cioè ad ogni click di suono
            counter++;
            return true;
        }

        private void Base1ApplicationBar_Click(object sender, EventArgs e)
        {
            VM.PlayBase("base1");
        }

        private void Base2ApplicationBar_Click(object sender, EventArgs e)
        {
            VM.PlayBase("base2");
        }

        private void Base3ApplicationBar_Click(object sender, EventArgs e)
        {
            VM.PlayBase("base3");
        }

        private void DisclaimerAppBarMenu_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/DisclaimerPage.xaml", UriKind.Relative));
        }

        private void AboutAppBarMenu_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

        private void Suggersci_Click(object sender, RoutedEventArgs e)
        {
            new EmailComposeTask()
            {
                Subject = "Suggerimento insulto SgarbiMix",
                To = "wpmobile@hotmail.it",
                Body = SuggerimentoTextBox.Text
            }.Show();
        }
    }
}