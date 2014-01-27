using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Media;
using SgarbiMix.WP7.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net.Http;
using System.Windows;
using WPCommon.Helpers;

namespace SgarbiMix.WP7.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private INavigationService _navigationService;
        private INavigationService NavigationService
        {
            get
            {
                if (_navigationService == null)
                    _navigationService = new NavigationService();
                return _navigationService;
            }
        }
        static IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();

        public event PropertyChangedEventHandler PropertyChanged;
        public IEnumerable<LLSGroup<string, SoundViewModel>> Sounds { get; set; }

        public MainViewModel()
        {
            if (DesignerProperties.IsInDesignTool)
            {
                var s = new SoundViewModel[]
                {
                    new SoundViewModel() { Name = "aoiadoaid", Category = "Corti" },
                    new SoundViewModel() { Name = "aoiadoaid", Category = "Corti" },
                    new SoundViewModel() { Name = "aoiadoaid", Category = "Corti" },
                    new SoundViewModel() { Name = "aoiadoaid", Category = "Corti" },
                    new SoundViewModel() { Name = "aoiadoaid", Category = "Corti" },
                    new SoundViewModel() { Name = "aoiadoaid", Category = "Lunghi" },
                    new SoundViewModel() { Name = "aoiadosaf asdfsadsa aid", Category = "Lunghi" },
                    new SoundViewModel() { Name = "aoiadoasfa sdfadf id", Category = "Lunghi" },
                    new SoundViewModel() { Name = "aoiaf asfsaas dfasf ddoaid", Category = "Lunghi" },
                };

                Sounds = from sound in s
                         group sound by sound.Category into g
                         select new LLSGroup<string, SoundViewModel>(g);
            }
            else
            {
                if (AppContext.AllSound == null) return;

                CheckUpdates();
                Sounds = from sound in AppContext.AllSound
                         group sound by sound.Category into g
                         select new LLSGroup<string, SoundViewModel>(g);
            }
        }

        private async void CheckUpdates()
        {
            using (var file = isf.OpenFile(AppContext.FilePath, FileMode.Open))
            using (var NewXml = await AppContext.GetNewXmlAsync())
                if (NewXml.Length == file.Length) return;


            var MsgBox = new CustomMessageBox()
            {
                Message = "Sono disponibili nuovi insulti, vuoi scaricarli?",
                LeftButtonContent = "Altroché!",
                RightButtonContent = "Ma sei scemo?"
            };

            MsgBox.Dismissed += (s1, e1) =>
            {
                if (e1.Result == CustomMessageBoxResult.LeftButton)
                    NavigationService.Navigate(new Uri("/View/UpdatePage.xaml", UriKind.Relative));
            };

            MsgBox.Show();
        }


        public static void PlayBase(string baseName)
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

        private static bool AskAndPlayMusic()
        {
            return MediaPlayer.GameHasControl ?
                true :
                MessageBox.Show("Vuoi interrompere la canzone corrente e riprodurre la base su cui mixare le frasi di Sgarbi?",
                    "SgarbiMix", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
        }

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
