using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Media;

namespace SheldonMix
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
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
    }
}