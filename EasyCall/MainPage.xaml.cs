using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using EasyCall.ViewModel;
using Microsoft.Phone.Controls;
using WPCommon.Helpers;
using System.Collections.Generic;
using System.Windows.Documents;

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
            tmr.Interval = TimeSpan.FromMilliseconds(100);
            tmr.Tick += (sender, e) =>
            {
                SearchTextBox.GetBindingExpression(
                    TextBox.TextProperty).UpdateSource();
                tmr.Stop();
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
                CallHelper.Call(VM.SearchedContacts.First().Model);
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

        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            //SCHIFEZZAAA!
            var number = ((Button)sender).DataContext.ToString();
            var contact = VM.SearchedContacts.Where(c => c.Model.Numbers.Contains(number)).First();
            CallHelper.Call(contact.Model);
        }
    }
}