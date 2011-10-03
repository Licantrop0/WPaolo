using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using EasyCall.ViewModel;
using Microsoft.Phone.Controls;
using WPCommon.Helpers;

namespace EasyCall
{
    public partial class MainPage : PhoneApplicationPage
    {
        DispatcherTimer tmr;

        MainViewModel _VM;
        public MainViewModel VM
        {
            get
            {
                if (_VM == null)
                    _VM = (MainViewModel)LayoutRoot.DataContext;

                return _VM;
            }
        }

        public MainPage()
        {
            InitializeComponent();
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            tmr = new DispatcherTimer();
            tmr.Interval = TimeSpan.FromMilliseconds(500);
            tmr.Tick += (sender, e) =>
            {
                tmr.Stop();
                SearchTextBox.GetBindingExpression(
                    TextBox.TextProperty).UpdateSource();
            };
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (TrialManagement.IsTrialMode)
                if (MessageBox.Show("Hi! Welcome to the Trial Mode.\nTo get rid of the nag screen and call limitations, press ok to buy this app.",
                    "Trial Mode", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    TrialManagement.Buy();

            SearchTextBox.Focus();
        }

        private void SearchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            tmr.Start();
        }

        private void SearchTextBox_ActionIconTapped(object sender, EventArgs e)
        {
            if (VM.SearchedContacts.Any())
            {
                var contact = VM.SearchedContacts.First();
                CallHelper.Call(contact.DisplayName, contact.Numbers.First());
            }
            else
            {
                CallHelper.Call(null, SearchTextBox.Text);
            }
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchTextBox.SelectAll();
        }

        private void About_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

        private void LongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ContactsLongListSelector.SelectedItem != null)
            {
                var number = (string)ContactsLongListSelector.SelectedItem;
                CallHelper.Call(null, number);
                ContactsLongListSelector.SelectedItem = null;
            }
        }
    }
}