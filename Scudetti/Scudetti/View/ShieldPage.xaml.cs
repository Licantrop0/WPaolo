using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using Scudetti.ViewModel;

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

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            var id = NavigationContext.QueryString["id"];
            VM.CurrentShield = AppContext.Shields.Single(s => s.Id == id);
            base.OnNavigatedTo(e);
        }
        
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            ShieldNameTextbox.Focus();
        }

        private void Ok_Click(object sender, EventArgs e)
        {
            ShieldNameTextbox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            VM.Validate();
        }

        private void ShieldNameTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Ok_Click(sender, EventArgs.Empty);
        }
    }
}