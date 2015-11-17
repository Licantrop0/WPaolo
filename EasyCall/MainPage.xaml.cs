using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using EasyCall.Helper;
using EasyCall.ViewModel;
using Microsoft.Phone.Controls;
using WPCommon.Helpers;

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
            SearchTextBox.Focus();

            if (TrialManagement.IsTrialMode)
            {
                AdMediator_BEB5C4.Visibility = Visibility.Visible;
                AdMediator_BEB5C4.IsEnabled = true;                
            }
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
            if (e.PlatformKeyCode == 190) // dot
            {
                e.Handled = true;
                SearchTextBox_OnActionIconTapped(sender, EventArgs.Empty);
            }
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!TrialManagement.IsTrialMode)
                return;

            e.Cancel = true;
            var messageBox = new CustomMessageBox()
            {
                Caption = "Trial Mode",
                Message = "To get rid of call limitations, buy this app",
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
                    App.Current.Terminate();
                }
            };
            messageBox.Show();
        }
    }
}