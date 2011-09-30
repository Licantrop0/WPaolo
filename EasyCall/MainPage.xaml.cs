using Microsoft.Phone.Controls;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System;

namespace EasyCall
{
    public partial class MainPage : PhoneApplicationPage
    {
        DispatcherTimer tmr;

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


        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            tmr.Start();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Focus();
        }
    }
}
