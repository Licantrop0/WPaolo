using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Soccerama.Win81.Helper;
using Windows.UI.Xaml.Navigation;

namespace Soccerama.Win81.View
{
    public sealed partial class LevelPage : Page
    {
        public LevelPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.DataContext = e.Parameter;
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                ResetPageCache();
            }
        }

        private void ResetPageCache()
        {
            var cacheSize = ((Frame)Parent).CacheSize;
            ((Frame)Parent).CacheSize = 0;
            ((Frame)Parent).CacheSize = cacheSize;
        }

        private void GoBack(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //TODO INSERIRE NAVIGAZIONE
        }
    }
}
