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
using System.Collections;

/* 
    Copyright (c) 2011 WPME
*/

namespace SgarbiMix
{
    public partial class PlaySoundPage : PhoneApplicationPage
    {
        Random rnd = new Random();
        private int counter;
        List<KeyValuePair<string, SoundEffect>> Sounds = new List<KeyValuePair<string, SoundEffect>>();

        public PlaySoundPage()
        {
            counter = 0;
            InitializeComponent();
            InitializeSounds();
            InitializeButtons();
            var sd = new ShakeDetector(3);
            sd.ShakeEvent += (sender, e) =>
            {
                Sounds[rnd.Next(Sounds.Count)].Value.Play();
            };
            sd.Start();
        }

        private void InitializeSounds()
        {
            var res = SoundsResources.ResourceManager.GetResourceSet(System.Globalization.CultureInfo.CurrentCulture, true, true);
            foreach (var resource in res)
            {
                var r = (DictionaryEntry)resource;
                Sounds.Add(new KeyValuePair<string, SoundEffect>(
                    r.Key.ToString().Replace("_", " "),
                    SoundEffect.FromStream((UnmanagedMemoryStream)r.Value)));
            }

            PlayButtonsListBox.ItemsSource = Sounds;

        }

        private void InitializeButtons()
        {
            var buttons = MainGrid.Children
                            .Where(c => c is Button)
                            .Cast<Button>().ToArray();

            for (int i = 0; i < buttons.Length; i++)
            {
                switch (i % 3)
                {
                    case 0: buttons[i].Background = new SolidColorBrush(Colors.Red); break;
                    case 1: buttons[i].Background = new SolidColorBrush(Colors.White); break;
                    case 2: buttons[i].Background = new SolidColorBrush(Colors.Green); break;
                }
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (CheckTrial())
            {
                var kvp = (KeyValuePair<string, SoundEffect>)((Button)sender).DataContext;
                kvp.Value.Play();
            }
        }

        private bool CheckTrial()
        {
            if (TrialManagement.IsTrialMode && TrialManagement.AlreadyOpenedToday && (counter > Sounds.Count))
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