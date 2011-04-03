using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using WPCommon;
using System.Diagnostics;
using System.Windows.Threading;


namespace SgarbiMix
{
    public partial class PlaySoundPage : PhoneApplicationPage
    {
        Random rnd = new Random();
        private int counter;

        //---------------------------------------------------
        private bool _bCanExecuteSound = true;
        private ShakeDetector sd = new ShakeDetector();
        private DispatcherTimer tmr = new DispatcherTimer();
        //------------------------------------------------

        private void OnTimerTick(object sender, EventArgs e)
        {
            tmr.Stop();
            _bCanExecuteSound = true;
        }

        public PlaySoundPage()
        {
            InitializeComponent();
            sd.ShakeDetected += (sender, e) =>
            {
                Dispatcher.BeginInvoke(() => { Sound_Shake(); });
            };
            sd.Start();

        }

        private void Sound_Shake()
        {

            if (_bCanExecuteSound)
                App.Sounds[rnd.Next(App.Sounds.Count)].Play();

            // ----------------------------------------------------
            _bCanExecuteSound = false;
            tmr.Interval = TimeSpan.FromMilliseconds(700);
            tmr.Tick += OnTimerTick;
            tmr.Start();
            //---------------------------------------------------- 

        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //se ho già caricato i bottoni esco
            if (PlayButtonsStackPanel.Children.Count > 0)
                return;

            //istanzio il BackgroundWorker (che gira in un thread separato)
            var bw = new BackgroundWorker() { WorkerReportsProgress = true };

            //Configuro l'evento DoWork
            bw.DoWork += (s, evt) =>
            {
                //prendo la risorsa dei suoni castandola in un array
                var sr = SoundsResources.ResourceManager
                    .GetResourceSet(CultureInfo.CurrentCulture, true, true)
                    .Cast<DictionaryEntry>()
                    .OrderBy(de => de.Key.ToString())
                    .ToArray();

                for (int i = 0; i < sr.Length; i++)
                {
                    //Aggiungo il suono alla lista dei suoni
                    App.Sounds.Add(new SoundContainer(sr[i].Key.ToString(), (UnmanagedMemoryStream)sr[i].Value));
                    bw.ReportProgress(i);
                }
            };

            //Creo il bottone corrispondende al suono non appena caricato
            bw.ProgressChanged += (s, evt) =>
            {
                var i = evt.ProgressPercentage;
                var b = new Button()
                {
                    //ATTENZIONE: mi baso sul nome del button per recuperare l'i-esimo sound
                    Name = i.ToString(),
                    Content = App.Sounds[i].ToString(),
                    Style = (Style)App.Current.Resources["PlayButtonStyle"]
                };

                b.Click += (button, evt1) =>
                {
                    if (CheckTrial())
                    {
                        //ATTENZIONE: mi baso sul nome del button per recuperare l'i-esimo sound
                        var SoundIndex = int.Parse(((Button)button).Name);
                        App.Sounds[SoundIndex].Play();
                    }
                };

                PlayButtonsStackPanel.Children.Add(b);
                //ButtonFade.Stop();
                //Storyboard.SetTarget(ButtonFade, b);
                //ButtonFade.Begin();
            };

            bw.RunWorkerAsync();
        }


        private bool CheckTrial()
        {
            if (TrialManagement.IsTrialMode && (App.alreadyOpen || (counter > (App.Sounds.Count / 4))))
            {
                NavigationService.Navigate(new Uri("/DemoPage.xaml", UriKind.Relative));
                return false;
            }
            //incremento ogni volta che chiamo checkTrial - cioè ad ogni click di suono
            counter++;
            return true;
        }


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
