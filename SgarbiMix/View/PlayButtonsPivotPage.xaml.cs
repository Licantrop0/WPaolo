﻿using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Advertising.Mobile.UI;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using SgarbiMix.ViewModel;
using WPCommon;
using WPCommon.Helpers;

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
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (TrialManagement.IsTrialMode)
                InitializeAd();
        }

        private void InitializeAd()
        {
            if (AdPlaceHolder.Children.Count == 1) //l'Ad c'è già
                return;

            var ad1 = new AdControl("c175f6ba-cb10-4fe3-a1de-a96480a03d3a", "10022581", true)
            {
                Height = 80,
                Width = 480
            };

            AdPlaceHolder.Children.Add(ad1);
        }

        private void InizializeShaker()
        {
            var sd = new MagnitudeDetector();
            sd.ShakeDetected += (sender, e) =>
            {                
                var snd = AppContext.GetRandomSound();
                Dispatcher.BeginInvoke(() =>
                {
                    snd.PlayCommand.Execute(null);
                });
                Thread.Sleep(snd.Duration + TimeSpan.FromMilliseconds(300)); //Questa sleep viene fatta nel thread dell'accelerometro, non blocca la UI
            };
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