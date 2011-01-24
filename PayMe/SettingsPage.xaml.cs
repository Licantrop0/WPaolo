using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Data;

namespace PayMe
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            CreateAppBar();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            SettingsStackPanel.DataContext = new Settings();
            //CurrencyListPicker.SelectedItem = Settings.CurrencySymbol;
            //HourlyPaymentTextBox.Text = GetCurrencyText(Settings.HourlyPayment);
            //CallPayTextBox.Text = GetCurrencyText(Settings.CallPay);
            //ThresholdSlider.Value = GetThresholdIndex(Settings.Threshold);
            //ThresholdTextBox.Text = GetThresholdText(ThresholdSlider.Value);
        }

        private void CreateAppBar()
        {
            ApplicationBar = new ApplicationBar();
            var SaveAppBarButton = new ApplicationBarIconButton();
            SaveAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_save.png", UriKind.Relative);
            SaveAppBarButton.Text = AppResources.Save;
            SaveAppBarButton.Click += delegate(object sender, EventArgs e)
            {
                HourlyPaymentTextBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                CallPayTextBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                ThresholdSlider.GetBindingExpression(Slider.ValueProperty).UpdateSource();
                //Settings.HourlyPayment = GetCurrencyValue(HourlyPaymentTextBox.Text);
                //Settings.CallPay = GetCurrencyValue(CallPayTextBox.Text);
                //Settings.Threshold = GetThresholdValue(ThresholdSlider.Value);
                NavigationService.GoBack();
            };

            ApplicationBar.Buttons.Add(SaveAppBarButton);
        }

        #region TextBox management

        private void HourlyPaymentTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            HourlyPaymentTextBox.SelectAll();
        }

        private void HourlyPaymentTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                CallPayTextBox.Focus();
        }

        private void CallPayTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            CallPayTextBox.SelectAll();
        }

        private void CallPayTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ThresholdSlider.Focus();
        }

        #endregion

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //if (!Settings.HourlyPayment.HasValue)
            //    throw new Exception("ForceExit");
        }

        //private void CurrencyListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    Settings.CurrencySymbol = ((ListPicker)sender).SelectedItem as string;
        //}
    }
}