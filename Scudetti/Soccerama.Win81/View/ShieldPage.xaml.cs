using Soccerama.Win81.ViewModel;
using System;
using System.Collections.Generic;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Soccerama.Win81.Helpers;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Soccerama.Win81.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShieldPage : Page
    {
        public ShieldPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            this.DataContext = e.Parameter;
            base.OnNavigatedTo(e);
        }

        private void ShieldNameTextbox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
                Ok_Click(sender, null);
        }

        private void LayoutAwarePage_Loaded(object sender, RoutedEventArgs e)
        {            
            ShieldNameTextbox.Focus(FocusState.Keyboard);
            ShieldNameTextbox.SelectAll();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            var VM = (ShieldViewModel)LayoutRoot.DataContext;
            if (!VM.Validate())
                ShieldNameTextbox.SelectAll();
        }

        private void Hint_Click(object sender, RoutedEventArgs e)
        {
            ShieldNameTextbox.Focus(FocusState.Keyboard);
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            //NAVIGATION BACK
        }
    }
}
