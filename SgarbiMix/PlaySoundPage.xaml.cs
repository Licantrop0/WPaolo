using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using WPCommon;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Collections;
using System.ComponentModel;
using Microsoft.Xna.Framework.Audio;
using System.Linq;


namespace SgarbiMix
{
    public partial class PlaySoundPage : PhoneApplicationPage
    {
        Random rnd = new Random();
        private int counter;
        object mutex = new object();

        public PlaySoundPage()
        {
            InitializeComponent();
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

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //se ho già caricato i bottoni esco
            if (PlayButtonsStackPanel.Children.Count > 0)
                return;

            //prendo la risorsa dei suoni castandola in un array
            var sr = SoundsResources.ResourceManager
                .GetResourceSet(CultureInfo.CurrentCulture, true, true)
                .Cast<DictionaryEntry>().ToArray();

            //istanzio il BackgroundWorker (che gira in un thread separato)
            var bw = new BackgroundWorker() { WorkerReportsProgress = true };

            //Configuro l'evento DoWork
            bw.DoWork += (s, evt) =>
            {
                for (int i = 0; i < sr.Length; i++)
                {
                    //Convenzione: "_" = spazio, "1" = punto esclamativo
                    var name = sr[i].Key.ToString().Replace("_", " ").Replace("1", "!");
                    var sound = (UnmanagedMemoryStream)sr[i].Value;

                    //Aggiungo il suono alla lista dei suoni
                    App.Sounds.Add(new KeyValuePair<string, SoundEffect>(name, SoundEffect.FromStream(sound)));
                    bw.ReportProgress(i);
                }
            };

            //Creo il bottone corrispondende al suono non appena caricato
            bw.ProgressChanged += (s, evt) =>
            {
                var i = evt.ProgressPercentage;
                var b = new Button();
                b.Content = App.Sounds[i].Key;
                b.Style = (Style)App.Current.Resources["PlayButtonStyle"];

                //ATTENZIONE cagata: mi baso sul nome del button per recuperare l'i-esimo sound
                b.Name = i.ToString();
                b.Click += (button, evt1) =>
                {
                    if (CheckTrial())
                    {
                        var SoundIndex = int.Parse(((Button)button).Name);
                        App.Sounds[SoundIndex].Value.Play();
                    }
                };
                PlayButtonsStackPanel.Children.Add(b);
            };

            bw.RunWorkerAsync();
        }


        private bool CheckTrial()
        {
            if (TrialManagement.IsTrialMode && (App.alreadyOpen || (counter > (App.Sounds.Count / 3))) )
            {
                NavigationService.Navigate(new Uri("/DemoPage.xaml", UriKind.Relative));
                return false;
            }
            //incremento ogni volta che chiamo checkTrial - cioè ad ogni click di suono
            counter++;
            return true;
        }

        //private void Base1ApplicationBar_Click(object sender, EventArgs e)
        //{
        //    if (Base1MediaElement.CurrentState == MediaElementState.Playing)
        //        Base1MediaElement.Stop();
        //    else
        //        Base1MediaElement.Play();
        //}

        //private void Base2ApplicationBar_Click(object sender, EventArgs e)
        //{
        //    if (Base2MediaElement.CurrentState == MediaElementState.Playing)
        //        Base2MediaElement.Stop();
        //    else
        //        Base2MediaElement.Play();
        //}

        //private void Base3ApplicationBar_Click(object sender, EventArgs e)
        //{
        //    if (Base3MediaElement.CurrentState == MediaElementState.Playing)
        //        Base3MediaElement.Stop();
        //    else
        //        Base3MediaElement.Play();
        //}

        //private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        //{
        //    var me = (MediaElement)sender;
        //    me.Stop();
        //    me.Play();
        //}

        private void DisclaimerApplicationBar_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/DisclaimerPage.xaml", UriKind.Relative));
        }

        private void AboutApplicationBar_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

    }
}
