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
        List<SoundContainerMP3> suoni;
        WPCommon.Helpers.XNAAsyncDispatcher musicXNa;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            suoni = new List<SoundContainerMP3>();
            musicXNa = new WPCommon.Helpers.XNAAsyncDispatcher();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);

        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            //se ho già caricato i bottoni esco
            if (ClassicPlayButtonsStackPanel.Children.Count > 0)
                return;

            WPCommon.Helpers.ExtensionMethods.StartTrace("carico risorse");
            //prendo la risorsa dei suoni castandola in un array
            var sr = SoundsResources.ResourceManager
                .GetResourceSet(CultureInfo.CurrentCulture, true, true)
                .Cast<DictionaryEntry>()
                .ToArray();
            WPCommon.Helpers.ExtensionMethods.EndTrace();

            //var bw = new BackgroundWorker() { WorkerReportsProgress = true };

            ////Configuro l'evento DoWork
            //bw.DoWork += (s, evt) =>
            //{
            //    WPCommon.Helpers.ExtensionMethods.StartTrace("ciclo sulla lista");
                for (int i = 0; i < sr.Length; i++)
                {
                    //Aggiungo il suono alla lista dei suoni
                    suoni.Add(new SoundContainerMP3(sr[i].Key.ToString()));
                    //bw.ReportProgress(i);
                }
            //    WPCommon.Helpers.ExtensionMethods.EndTrace();
            //};

            //Creo il bottone corrispondende al suono non appena caricato
            for (int i = 0; i < sr.Length; i++)
            {
                var text = suoni[i].ToString();
                var type = suoni[i].Type();
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
                    suoni[SoundIndex].Play();
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

            //bw.RunWorkerAsync();
            
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