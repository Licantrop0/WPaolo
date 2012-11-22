using SocceramaWin8.Common;
using SocceramaWin8.ViewModel;
using System;
using System.Collections.Generic;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SocceramaWin8.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShieldPage : LayoutAwarePage
    {
        public ShieldPage()
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

        private void ShieldNameTextbox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
                Ok_Click(sender, null);
        }

        private void LayoutAwarePage_Loaded(object sender, RoutedEventArgs e)
        {            
            ShieldNameTextbox.Focus(FocusState.Keyboard);
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            var VM = (ShieldViewModel)LayoutRoot.DataContext;
            //if (!VM.Validate())
            //    ShieldNameTextbox.SelectAll();
        }
    }
}
