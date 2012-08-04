using System;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Coding4Fun.Phone.Controls;
using Microsoft.Phone.Controls;
using Scudetti.Localization;
using Scudetti.Sound;

namespace Scudetti.View
{
    public partial class ShieldsPage : PhoneApplicationPage
    {
        public ShieldsPage()
        {
            InitializeComponent();
        }

        private bool toastTapped;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            toastTapped = false;

            var levelIndex = int.Parse(NavigationContext.QueryString["level"]);
            LayoutRoot.DataContext = AppContext.Levels.Single(l => l.Number == levelIndex);

            var newLevelTreshold = AppContext.TotalShieldUnlocked % AppContext.LockTreshold;
            int levelNumber = AppContext.TotalShieldUnlocked / AppContext.LockTreshold;

            if (e.NavigationMode != NavigationMode.Back) return;
            if (AppContext.TotalShieldUnlocked == 0) return;
            if (AppContext.ToastDisplayed)
            {
                AppContext.ToastDisplayed = newLevelTreshold == 0;
                return;
            }

            if (newLevelTreshold == 0 && levelNumber < 6)
            {
                SoundManager.PlayGoal();
                var toast = new ToastPrompt
                {
                    Message = string.Format(AppResources.NewLevel, levelNumber + 1),
                    ImageSource = new BitmapImage(new Uri("..\\Images\\soccer icon.png", UriKind.Relative))
                };
                toast.Tap += (s1, e1) =>
                {
                    toastTapped = true;
                    NavigationService.Navigate(new Uri("/View/ShieldsPage.xaml?level=" + (levelNumber + 1), UriKind.Relative));
                };
                toast.Show();
                AppContext.ToastDisplayed = true;
            }

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if(toastTapped) NavigationService.RemoveBackEntry();
        }

    }
}