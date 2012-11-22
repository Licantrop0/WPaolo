using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using SocceramaWin8.Helper;

namespace SocceramaWin8.View
{
    public sealed partial class ShieldsPage : SocceramaWin8.Common.LayoutAwarePage
    {
        ScrollViewer gridScrollViewer;

        public ShieldsPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        protected override void OnNavigatedFrom(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            AppContext.ShieldsScrollPosition = gridScrollViewer.HorizontalOffset;
            base.OnNavigatedFrom(e);
        }
 
        private void itemGridView_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.gridScrollViewer = VisualHelper.FindVisualChild<ScrollViewer>(this.itemGridView);
            this.gridScrollViewer.ScrollToHorizontalOffset(AppContext.ShieldsScrollPosition);
        }
    }
}
