using System;
using System.IO.IsolatedStorage;
using System.Windows;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework.Media;
using DeathTimerz.Localization;

namespace DeathTimerz
{
    public static class Settings
    {
        public const double AverageYear = 365.25;
        public const double AverageMonth = 30.4368499;

        public static DateTime? BirthDay
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("birthday"))
                    IsolatedStorageSettings.ApplicationSettings["birthday"] = null;
                return (DateTime?)IsolatedStorageSettings.ApplicationSettings["birthday"];
            }
            set
            {
                if (BirthDay != value)
                    IsolatedStorageSettings.ApplicationSettings["birthday"] = value;
            }
        }

        public static TimeSpan? EstimatedDeathAge
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("estimated_death_age"))
                    IsolatedStorageSettings.ApplicationSettings["estimated_death_age"] = null;
                return (TimeSpan?)IsolatedStorageSettings.ApplicationSettings["estimated_death_age"];
            }
            set
            {
                if (EstimatedDeathAge != value)
                    IsolatedStorageSettings.ApplicationSettings["estimated_death_age"] = value;
            }
        }

        public static void AskAndPlayMusic()
        {
            if (!PhoneApplicationService.Current.State.ContainsKey("can_play_music"))
            {
                PhoneApplicationService.Current.State.Add("can_play_music", true);
                PhoneApplicationService.Current.State["can_play_music"] =
                    MediaPlayer.GameHasControl ?
                    true :
                    MessageBox.Show(AppResources.PlayBackgroundMusic,
                        AppResources.AppName, MessageBoxButton.OKCancel) == MessageBoxResult.OK;
            }

            if ((bool)PhoneApplicationService.Current.State["can_play_music"])
            {
                var BackgroundMusic = Song.FromUri("BackgroudMusic", new Uri("Music.wma", UriKind.Relative));
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(BackgroundMusic);
            }
        }
    }
}
