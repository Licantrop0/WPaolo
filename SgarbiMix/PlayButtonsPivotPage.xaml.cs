using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Media;
using System.ComponentModel;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System.Globalization;
using System.Collections;
using System.IO;

namespace SgarbiMix
{
    public partial class PlayButtonsPivotPage : PhoneApplicationPage
    {
        public PlayButtonsPivotPage()
        {
            InitializeComponent();
            if (WPCommon.TrialManagement.IsTrialMode)
            {
                AdPlaceHolder.Children.Add(new AdDuplex.AdControl() { AppId = "", IsTest = true });
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

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //se ho già caricato i bottoni esco
            if (ShortPlayButtonsStackPanel.Children.Count > 0)
                return;

            WPCommon.ExtensionMethods.StartTrace("carico risorse");
            //prendo la risorsa dei suoni castandola in un array ordinato
            var sr = SoundsResources.ResourceManager
                .GetResourceSet(CultureInfo.CurrentCulture, true, true)
                .Cast<DictionaryEntry>()
                .OrderBy(de => de.Key.ToString().Length)
                .ToArray();
            WPCommon.ExtensionMethods.EndTrace();

            var bw = new BackgroundWorker() { WorkerReportsProgress = true };

            //Configuro l'evento DoWork
            bw.DoWork += (s, evt) =>
            {
                WPCommon.ExtensionMethods.StartTrace("ciclo sulla lista");
                for (int i = 0; i < sr.Length; i++)
                {
                    //Aggiungo il suono alla lista dei suoni
                    App.Sounds.Add(new SoundContainer(sr[i].Key.ToString(), (UnmanagedMemoryStream)sr[i].Value));
                    bw.ReportProgress(i);
                }
                WPCommon.ExtensionMethods.EndTrace();
            };

            //Creo il bottone corrispondende al suono non appena caricato
            bw.ProgressChanged += (s, evt) =>
            {
                var i = evt.ProgressPercentage;
                var text = App.Sounds[i].ToString();
                var b = new Button()
                {
                    Tag = i, //ATTENZIONE: creo un tag per recuperare l'i-esimo sound
                    Content = text,
                    Width = text.Length > 18 ? 468 : 230,
                    Style = (Style)App.Current.Resources["PlayButtonStyle"]
                };

                b.Click += (button, evt1) =>
                {
                    //ATTENZIONE: uso il tag per recuperare l'i-esimo sound
                    var SoundIndex = (int)((Button)button).Tag;
                    App.Sounds[SoundIndex].Play();
                };

                if (b.Width > 400)
                    LongPlayButtonsStackPanel.Children.Add(b);
                else
                    ShortPlayButtonsStackPanel.Children.Add(b);
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