using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Audio;
using WPCommon;
using System.IO;
using System.Collections.Generic;

/* 
    Copyright (c) 2011 WPME
*/

namespace SgarbiMix
{
    public partial class PlaySoundPage : PhoneApplicationPage
    {
        Random rnd = new Random();
        private int counter;
        object mutex = new object();

        public PlaySoundPage()
        {
            counter = 0;
            InitializeComponent();
            InitializeButtons();
            var sd = new ShakeDetector();
            sd.ShakeDetected += (sender, e) =>
            {
                lock (mutex) //è necessario?
                {
                    App.Sounds[rnd.Next(App.Sounds.Count)].Value.Play();
                }
            };
            sd.Start();
        }


        private void InitializeButtons()
        {
            for (int i = 0; i < App.Sounds.Count; i++)
            {
                var b = new Button();
                b.Content = App.Sounds[i].Key;
                b.Style = (Style)App.Current.Resources["PlayButtonStyle"];
                switch (i % 3)
                {
                    case 0: b.Background = new SolidColorBrush(Colors.Red); break;
                    case 1: b.Background = new SolidColorBrush(Colors.White); break;
                    case 2: b.Background = new SolidColorBrush(Colors.Green); break;
                }
                //ATTENZIONE cagata: mi baso sul nome del button per recuperare l'iesimo sound
                b.Name = i.ToString();
                b.Click += (sender, e) =>
                {
                    if (CheckTrial())
                    {
                        var SoundIndex = int.Parse(((Button)sender).Name);
                        App.Sounds[SoundIndex].Value.Play();
                    }
                };
                PlayButtonsStackPanel.Children.Add(b);
            }
        }

        private bool CheckTrial()
        {
            if (TrialManagement.IsTrialMode && TrialManagement.AlreadyOpenedToday && (counter > App.Sounds.Count))
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
            if (Base1MediaElement.CurrentState == MediaElementState.Playing)
                Base1MediaElement.Stop();
            else
                Base1MediaElement.Play();
        }

        private void Base2ApplicationBar_Click(object sender, EventArgs e)
        {
            if (Base2MediaElement.CurrentState == MediaElementState.Playing)
                Base2MediaElement.Stop();
            else
                Base2MediaElement.Play();
        }

        private void Base3ApplicationBar_Click(object sender, EventArgs e)
        {
            if (Base3MediaElement.CurrentState == MediaElementState.Playing)
                Base3MediaElement.Stop();
            else
                Base3MediaElement.Play();
        }

        private void DisclaimerApplicationBar_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/DisclaimerPage.xaml", UriKind.Relative));
        }

        private void AboutApplicationBar_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            var me = (MediaElement)sender;
            me.Stop();
            me.Play();
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            throw new Exception("ForceExit");
        }
    }
}