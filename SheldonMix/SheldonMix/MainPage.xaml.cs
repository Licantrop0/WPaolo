using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Media;
using System.Net;
using Microsoft.Phone.Tasks;

namespace SheldonMix
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void AboutAppBarMenu_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
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

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { Uri = new Uri("http://www.youtube.com/watch?v=SifGskrY_UY") }.Show();
        }

        private void HyperlinkButton_Click_1(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { Uri = new Uri("http://twitter.com/#!/BigBang_CBS") }.Show();
        }

        private void HyperlinkButton_Click_2(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { Uri = new Uri("http://www.cbs.com/shows/big_bang_theory") }.Show();    
        }

        private void Suggersci_Click(object sender, RoutedEventArgs e)
        {
            new EmailComposeTask()
            {
                Subject = "[SheldonMix] Feedback and request for upgrades",
                To = "wpmobile@hotmail.it",
                Body = SuggerimentoTextBox.Text
            }.Show();
        }

        private void HyperlinkButton_Click_3(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { Uri = new Uri("http://www.imdb.com/name/nm1433588/") }.Show();       
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            try
            {
                new MarketplaceReviewTask().Show();
            }
            catch (InvalidOperationException)
            { /*do nothing */ }
        }
    }
}