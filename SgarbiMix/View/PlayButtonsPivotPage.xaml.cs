using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using SgarbiMix.ViewModel;
using WPCommon;
using WPCommon.Helpers;
using Microsoft.Advertising.Mobile.UI;

namespace SgarbiMix
{
    public partial class PlayButtonsPivotPage : PhoneApplicationPage
    {
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

            if (TrialManagement.IsTrialMode)
                InitializeAd();
        }

        private void InitializeAd()
        {
            var ad1 = new AdControl("c175f6ba-cb10-4fe3-a1de-a96480a03d3a", "10022581", true)
            {
                Height = 80,
                Width = 480
            };

            ad1.SetValue(Grid.RowProperty, 1);
            LayoutRoot.Children.Add(ad1);
        }

        private void InizializeShaker()
        {
            var sd = new ShakeDetector();
            var rnd = new Random();

            sd.ShakeDetected += (sender, e) =>
            {
                var RandomSound = VM.SoundResources[rnd.Next(VM.SoundResources.Length)];
                RandomSound.PlayCommand.Execute(null);
            };

            sd.Start();
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
            NavigationService.Navigate(new Uri("/View/DisclaimerPage.xaml", UriKind.Relative));
        }

        private void AboutAppBarMenu_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/AboutPage.xaml", UriKind.Relative));
        }

        private void Suggersci_Click(object sender, RoutedEventArgs e)
        {
            new EmailComposeTask()
            {
                Subject = "[SgarbiMix] Suggerimento insulto",
                To = "wpmobile@hotmail.it",
                Body = SuggerimentoTextBox.Text
            }.Show();
        }
    }
}