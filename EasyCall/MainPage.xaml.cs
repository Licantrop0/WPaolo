using EasyCall.ViewModel;
using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using WPCommon.Helpers;
using System.Linq;
using EasyCall.Helper;

namespace EasyCall
{
    public partial class MainPage : PhoneApplicationPage
    {
        private DispatcherTimer _tmr;

        private MainViewModel _vm;
        public MainViewModel Vm => _vm ?? (_vm = (MainViewModel)LayoutRoot.DataContext);

        public MainPage()
        {
            InitializeComponent();
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            _tmr = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
            _tmr.Tick += (sender, e) =>
            {
                _tmr.Stop();
                SearchTextBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            };
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!TrialManagement.IsTrialMode)
            {
                AdMediator_BEB5C4.Disable();
                AdMediator_BEB5C4.Visibility = Visibility.Collapsed;
                return;
            }
            var messageBox = new CustomMessageBox()
            {
                Caption = "Welcome to the Trial Mode",
                Message = "Hi there, to get rid of this nag screen and call limitations, press Buy.",
                LeftButtonContent = "Buy",
                RightButtonContent = "Maybe later",
            };

            messageBox.Dismissed += (s1, e1) =>
            {
                if (e1.Result == CustomMessageBoxResult.LeftButton)
                {
                    TrialManagement.Buy();
                }
                else
                {
                    SearchTextBox.Focus();
                }
            };

            messageBox.Show();
        }

        private void About_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

        private void SearchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            _tmr.Start();
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchTextBox.SelectAll();
        }

        private void ContactsLongListSelector_OnManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            //Dismiss the keyboard
            if (FocusManager.GetFocusedElement() == SearchTextBox)
                this.Focus();
        }

        private void SearchTextBox_OnActionIconTapped(object sender, EventArgs e)
        {
            if (Vm.SearchedContacts.Any())
            {
                Vm.SearchedContacts.First().First().CallNumberCommand.Execute(null);
            }
            else
            {
                CallHelper.Call(null, SearchTextBox.Text);
            }

        }

        private void SearchTextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Decimal)
            {
                SearchTextBox_OnActionIconTapped(sender, EventArgs.Empty);
            }
        }
    }
}