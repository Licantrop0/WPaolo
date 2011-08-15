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

namespace SheldonMix
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);

            // no advertisement
            //if (WPCommon.Helpers.TrialManagement.IsTrialMode)
            //{
            //    AdPlaceHolder.Children.Add(new AdDuplex.AdControl() { AppId = "", IsTest = true });
            //    var BuyFullMenuItem = new ApplicationBarMenuItem("Rimuovi la pubblicità");
            //    BuyFullMenuItem.Click += (sender, e) =>
            //    {
            //        //Rimando a Sgarbimix senza Ad a pagamento
            //        var detailTask = new MarketplaceDetailTask() { ContentIdentifier = "5925f9d6-483d-e011-854c-00237de2db9e" };
            //        detailTask.Show();
            //    };
            //    ApplicationBar.MenuItems.Add(BuyFullMenuItem);
            //}
        }

        //// Load data for the ViewModel Items
        //private void MainPage_Loaded(object sender, RoutedEventArgs e)
        //{
        //    if (!App.ViewModel.IsDataLoaded)
        //    {
        //        App.ViewModel.LoadData();
        //    }
        //}

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            //se ho già caricato i bottoni esco
            if (ClassicPlayButtonsStackPanel.Children.Count > 0)
                return;

            WPCommon.Helpers.ExtensionMethods.StartTrace("carico risorse");
            //prendo la risorsa dei suoni castandola in un array ordinato
            var sr = SoundsResources.ResourceManager
                .GetResourceSet(CultureInfo.CurrentCulture, true, true)
                .Cast<DictionaryEntry>()
                .OrderBy(de => de.Key.ToString().Length)
                .ToArray();
            WPCommon.Helpers.ExtensionMethods.EndTrace();

            var bw = new BackgroundWorker() { WorkerReportsProgress = true };

            //Configuro l'evento DoWork
            bw.DoWork += (s, evt) =>
            {
                WPCommon.Helpers.ExtensionMethods.StartTrace("ciclo sulla lista");
                for (int i = 0; i < sr.Length; i++)
                {
                    //Aggiungo il suono alla lista dei suoni
                    //App.Sounds.Add(new SoundContainer(sr[i].Key.ToString(), (UnmanagedMemoryStream)sr[i].Value));
                    App.Sounds.Add(new SoundContainerMP3(sr[i].Key.ToString()));
                    bw.ReportProgress(i);
                }
                WPCommon.Helpers.ExtensionMethods.EndTrace();
            };

            //Creo il bottone corrispondende al suono non appena caricato
            bw.ProgressChanged += (s, evt) =>
            {
                var i = evt.ProgressPercentage;
                var text = App.Sounds[i].ToString();
                var type = App.Sounds[i].Type();
                var b = new Button()
                {
                    Tag = i, //ATTENZIONE: creo un tag per recuperare l'i-esimo sound
                    Content = text,
                    Width = 468, //dimensione fissa per tutti
                    Style = (Style)App.Current.Resources["PlayButtonStyle"]
                };

                b.Click += (button, evt1) =>
                {
                    //ATTENZIONE: uso il tag per recuperare l'i-esimo sound
                    var SoundIndex = (int)((Button)button).Tag;
                    App.Sounds[SoundIndex].Play();
                };

                if (type == 1)
                    BazingaPlayButtonsStackPanel.Children.Add(b);
                else if (type == 2)
                    ClassicPlayButtonsStackPanel.Children.Add(b);
                else if (type == 3)
                    TBBTPlayButtonsStackPanel.Children.Add(b);
                else
                { /*some error */}
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
                MessageBox.Show("Do you want to stop your music and play the music to mix Sheldon?",
                    "SheldonMix", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
        }

        private void AboutAppBarMenu_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }
    }
}