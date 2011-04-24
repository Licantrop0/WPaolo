using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework.Media;
using WPCommon;
using Google.AdMob.Ads.WindowsPhone7.WPF;
using Microsoft.Phone.Tasks;

namespace SgarbiMix
{
    public partial class PlaySoundPage : PhoneApplicationPage
    {
        Random rnd = new Random();

        //---------------------------------------------------
        private bool _bCanExecuteSound = true;
        private ShakeDetector sd = new ShakeDetector();
        private DispatcherTimer tmr = new DispatcherTimer();
        //------------------------------------------------

        public PlaySoundPage()
        {
            InitializeComponent();

            tmr.Interval = TimeSpan.FromMilliseconds(700);
            tmr.Tick += (sender, e) =>
            {
                tmr.Stop();
                _bCanExecuteSound = true;
            };

            sd.ShakeDetected += (sender, e) =>
            {
                Dispatcher.BeginInvoke(() => { Sound_Shake(); });
            };
            sd.Start();

            if (WPCommon.TrialManagement.IsTrialMode)
            {
                MainGrid.Children.Add(new BannerAd() { AdUnitID = "a14da0253ee7df0" });
                var BuyFullMenuItem = new ApplicationBarMenuItem("Rimuovi la pubblicità");
                BuyFullMenuItem.Click += (sender, e) =>
                {
                    //Rimando a Sgarbimix senza Ad a pagamento
                    var detailTask = new MarketplaceDetailTask() { ContentIdentifier = "5925f9d6-483d-e011-854c-00237de2db9e" };
                    detailTask.Show();
                };
                ApplicationBar.MenuItems.Add(BuyFullMenuItem);
            }
        }

        private void Sound_Shake()
        {
            if (_bCanExecuteSound)
                App.Sounds[rnd.Next(App.Sounds.Count)].Play();

            // ----------------------------------------------------
            _bCanExecuteSound = false;
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
                //prendo la risorsa dei suoni castandola in un array ordinato
                var sr = SoundsResources.ResourceManager
                    .GetResourceSet(CultureInfo.CurrentCulture, true, true)
                    .Cast<DictionaryEntry>()
                    .OrderBy(de => de.Key.ToString().Length)
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
                    Tag = i, //ATTENZIONE: creo un tag per recuperare l'i-esimo sound
                    Content = App.Sounds[i].ToString(),
                    Style = (Style)App.Current.Resources["PlayButtonStyle"]
                };

                b.Click += (button, evt1) =>
                {
                    //ATTENZIONE: uso il tag per recuperare l'i-esimo sound
                    var SoundIndex = (int)((Button)button).Tag;
                    App.Sounds[SoundIndex].Play();
                };

                PlayButtonsStackPanel.Children.Add(b);
            };

            bw.RunWorkerAsync();
        }

        private void Base1ApplicationBar_Click(object sender, EventArgs e)
        {
            PlayBase("base1");
        }

        private void Base2ApplicationBar_Click(object sender, EventArgs e)
        {
            PlayBase("base2");
        }

        private void Base3ApplicationBar_Click(object sender, EventArgs e)
        {
            PlayBase("base3");
        }

        private static void PlayBase(string baseName)
        {
            if (AskAndPlayMusic())
            {
                if (MediaPlayer.Queue.Count == 1 && MediaPlayer.Queue.ActiveSong.Name == baseName && MediaPlayer.State == MediaState.Playing)
                    MediaPlayer.Stop();
                else
                {
                    MediaPlayer.Play(Song.FromUri(baseName, new Uri("sounds/" + baseName + ".mp3", UriKind.Relative)));
                    MediaPlayer.IsRepeating = true;
                }
            }
        }

        public static bool AskAndPlayMusic()
        {
            return MediaPlayer.GameHasControl ?
                true :
                MessageBox.Show("Vuoi interrompere la canzone corrente e riprodurre la base su cui mixare le frasi di Sgarbi?",
                    "SgarbiMix", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
        }

        private void DisclaimerAppBarMenu_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/DisclaimerPage.xaml", UriKind.Relative));
        }

        private void AboutAppBarMenu_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }
    }
}
