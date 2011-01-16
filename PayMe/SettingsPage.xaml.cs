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
            HourlyPaymentTextBox.Text = GetCurrencyText(Settings.HourlyPayment);
            CallPayTextBox.Text = GetCurrencyText(Settings.CallPay);
            ThresholdSlider.Value = GetThresholdIndex(Settings.Threshold);
            ThresholdTextBox.Text = GetThresholdText(ThresholdSlider.Value);
        }

        private void ThresholdSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ThresholdTextBox.Text = GetThresholdText(e.NewValue);
        }

        private void CreateAppBar()
        {
            ApplicationBar = new ApplicationBar();
            var SaveAppBarButton = new ApplicationBarIconButton();
            SaveAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar_save.png", UriKind.Relative);
            SaveAppBarButton.Text = AppResources.Save;
            SaveAppBarButton.Click += delegate(object sender, EventArgs e)
            {
                Settings.HourlyPayment = GetCurrencyValue(HourlyPaymentTextBox.Text);
                Settings.CallPay = GetCurrencyValue(CallPayTextBox.Text);
                Settings.Threshold = GetThresholdValue(ThresholdSlider.Value);
                NavigationService.GoBack();
            };

            ApplicationBar.Buttons.Add(SaveAppBarButton);
        }

        #region Converters

        double GetCurrencyValue(string currency)
        {
            double temp;
            return double.TryParse(
                currency.Replace(" " + Settings.CurrencySymbol, string.Empty), out temp) ? temp : 0.0;
        }

        string GetCurrencyText(double? currency)
        {
            if (!currency.HasValue) currency = 0.0;
            return string.Format("{0:0.00} {1}", currency, Settings.CurrencySymbol);
        }


        private TimeSpan GetThresholdValue(double index)
        {
            switch (Convert.ToInt32(index))
            {
                case 1:
                    return TimeSpan.FromMinutes(5);
                case 2:
                    return TimeSpan.FromMinutes(10);
                case 3:
                    return TimeSpan.FromMinutes(15);
                case 4:
                    return TimeSpan.FromMinutes(30);
                case 5:
                    return TimeSpan.FromMinutes(60);
                default:
                    return TimeSpan.FromSeconds(1);
            }
        }

        private int GetThresholdIndex(TimeSpan t)
        {
            switch (Convert.ToInt32(t.TotalSeconds))
            {
                case 5 * 60:
                    return 1;
                case 10 * 60:
                    return 2;
                case 15 * 60:
                    return 3;
                case 30 * 60:
                    return 4;
                case 60 * 60:
                    return 5;
                default:
                    return 0;
            }
        }

        private string GetThresholdText(double value)
        {
            switch (Convert.ToInt32(value))
            {
                case 1:
                    return string.Format("{0}: {1}", AppResources.Threshold, AppResources.FiveMinutes);
                case 2:
                    return string.Format("{0}: {1}", AppResources.Threshold, AppResources.TenMinutes);
                case 3:
                    return string.Format("{0}: {1}", AppResources.Threshold, AppResources.FifteenMinutes);
                case 4:
                    return string.Format("{0}: {1}", AppResources.Threshold, AppResources.HalfHour);
                case 5:
                    return string.Format("{0}: {1}", AppResources.Threshold, AppResources.OneHour);
                default:
                    return string.Format("{0}: {1}", AppResources.Threshold, AppResources.OneSecond);
            }
        }

        #endregion

        #region TextBox management

        private void HourlyPaymentTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            HourlyPaymentTextBox.Text = GetCurrencyText(
                GetCurrencyValue(HourlyPaymentTextBox.Text));
        }

        private void HourlyPaymentTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            HourlyPaymentTextBox.SelectAll();
        }

        private void HourlyPaymentTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                CallPayTextBox.Focus();
        }

        private void CallPayTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            CallPayTextBox.Text = GetCurrencyText(
                GetCurrencyValue(CallPayTextBox.Text));
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
            if (!Settings.HourlyPayment.HasValue)
                throw new Exception("ForceExit");
        }
    }
}