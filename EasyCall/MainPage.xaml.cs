using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using EasyCall.ViewModel;
using Microsoft.Phone.Controls;

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
                    _VM = (MainViewModel)this.DataContext;

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
                VM.Call(contact.DisplayName, contact.Numbers.First());
            }
            else
            {
                VM.Call(null, SearchTextBox.Text);
            }
        }

        private void Call_Click(object sender, RoutedEventArgs e)
        {
            var num = ((Button)sender).DataContext as string;
            VM.Call(null, num);
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchTextBox.SelectAll();
        }
    }
}
