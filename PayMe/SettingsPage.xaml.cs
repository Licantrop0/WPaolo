using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.ComponentModel;
using Converters;

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

                NavigationService.GoBack();
            };

            ApplicationBar.Buttons.Add(SaveAppBarButton);
        }

        #region TextBox management

        private void HourlyPaymentTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            HourlyPaymentTextBox.SelectAll();
        }

        private void HourlyPaymentTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            HourlyPaymentTextBox.Text = CurrencyTextConverter.GetCurrencyText(
                CurrencyTextConverter.GetCurrencyValue(HourlyPaymentTextBox.Text, CultureInfo.CurrentCulture));
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

        private void CallPayTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            CallPayTextBox.Text = CurrencyTextConverter.GetCurrencyText(
                CurrencyTextConverter.GetCurrencyValue(CallPayTextBox.Text, CultureInfo.CurrentCulture));
        }

        private void CallPayTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ThresholdSlider.Focus();
        }

        #endregion

        private void PhoneApplicationPage_BackKeyPress(object sender, CancelEventArgs e)
        {
            if (!new Settings().HourlyPayment.HasValue)
                throw new Exception("ForceExit");
        }

        //private void CurrencyListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    Settings.CurrencySymbol = ((ListPicker)sender).SelectedItem as string;
        //}
    }
}