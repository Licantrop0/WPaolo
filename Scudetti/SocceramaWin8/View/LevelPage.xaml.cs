using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using SocceramaWin8.Helper;
using Windows.UI.Xaml.Navigation;

namespace SocceramaWin8.View
{
    public sealed partial class LevelPage : SocceramaWin8.Common.LayoutAwarePage
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
    }
}
