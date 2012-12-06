using SocceramaWin8.Common;
using System;
using System.Collections.Generic;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SocceramaWin8.Presentation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShieldView : LayoutAwarePage
    {
        public ShieldView()
        {
            this.InitializeComponent();
        }

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
            if (!VM.Validate())
                ShieldNameTextbox.SelectAll();
        }
    }
}
