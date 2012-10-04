using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Advertising.Mobile.UI;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Scudetti.ViewModel;
using System.Windows.Navigation;

namespace Scudetti.View
{
    public partial class ShieldPage : PhoneApplicationPage
    {
        private ShieldViewModel _vM;
        private ShieldViewModel VM
        {
            get { return _vM ?? (_vM = LayoutRoot.DataContext as ShieldViewModel); }
        }

        public ShieldPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (AppContext.AvailableHints > 0 && !VM.CurrentShield.IsValidated)
                ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = true;

            if (AdPlaceHolder.Children.Count == 1) //l'Ad c'è già
                return;

            AdPlaceHolder.Children.Add(new AdControl("489510bc-7ee7-4d2d-9bf1-9065ff63354d", "10040107", true)
            {
                Height = 80,
                Width = 480,
            });
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            AdPlaceHolder.Children.Clear();
            base.OnNavigatedFrom(e);
        }
        
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            ShieldNameTextbox.Focus();
        }

        private void Ok_Click(object sender, EventArgs e)
        {
            ShieldNameTextbox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            if (!VM.Validate())
                ShieldNameTextbox.SelectAll();
        }

        private void ShieldNameTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Ok_Click(sender, EventArgs.Empty);
        }

        private void Hint_Click(object sender, EventArgs e)
        {
            VM.ShowHint();
            ((ApplicationBarIconButton)sender).IsEnabled = false;
        }
    }
}