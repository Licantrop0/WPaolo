using System;
using System.Windows.Media.Imaging;
using Coding4Fun.Phone.Controls;
using Microsoft.Phone.Controls;
using System.Windows.Navigation;
using Scudetti.Localization;

namespace Scudetti.View
{
    public partial class ShieldsPage : PhoneApplicationPage
    {
        public ShieldsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var levelIndex = int.Parse(NavigationContext.QueryString["level"]);
            LayoutRoot.DataContext = AppContext.Levels[levelIndex];

            if (AppContext.TotalShieldUnlocked != 0 && AppContext.TotalShieldUnlocked % AppContext.LockTreshold == 0)
            {
                new ToastPrompt
                {
                    Message = AppResources.NewLevel,
                    ImageSource = new BitmapImage(new Uri("..\\ApplicationIcon.png", UriKind.Relative))
                }.Show();
            }

            base.OnNavigatedTo(e);
        }
    }
}