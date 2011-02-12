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
        private int counter;
        List<SoundEffect> sounds = new List<SoundEffect>();
        Random rnd;

        public PlaySoundPage()
        {
            counter = 0;
            InitializeComponent();
            InitializeSounds();
            InitializeButtons();
            var sd = new ShakeDetector();
            sd.ShakeEvent += (sender, e) =>
            {
                sounds[rnd.Next(sounds.Count)].Play();
            };
            sd.Start();
        }

        private void InitializeSounds()
        {
            foreach (var wav in Directory.GetFiles("sounds", "*.wav"))
            {
                sounds.Add(SoundEffect.FromStream(App.GetResourceStream(new Uri(wav, UriKind.Relative)).Stream));
            }
        }

        private void InitializeButtons()
        {
            var soundFiles = Directory.GetFiles("sounds", "*.wav");
            for (int i = 0; i < soundFiles.Length; i++)
            {
                var b = new Button();
                b.Style = (Style)Resources["PlayButtonStyle"];
                b.Content = Path.GetFileNameWithoutExtension(soundFiles[i]);
                switch (i % 3)
                {
                    case 0: b.Background = new SolidColorBrush(Colors.Red); break;
                    case 1: b.Background = new SolidColorBrush(Colors.White); break;
                    case 2: b.Background = new SolidColorBrush(Colors.Green); break;
                }
                b.Click += (sender, e) =>
                {
                    if (CheckTrial())
                        sounds[i].Play();
                };

                ButtonsStackPanel.Children.Add(b);
            }
        }

        private bool CheckTrial()
        {
            if (TrialManagement.IsTrialMode && TrialManagement.AlreadyOpenedToday && (counter > 22))
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

    }
}