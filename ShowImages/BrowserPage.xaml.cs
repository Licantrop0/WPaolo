using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using HtmlAgilityPack;
using Microsoft.Phone.Controls;

namespace ShowImages
{
    public partial class BrowserPage : PhoneApplicationPage
    {
        public BrowserPage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            ChooserWebBrowser.Navigate(Settings.BrowserHistory.Pop());
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Settings.BrowserHistory.IsEmpty) return;
            e.Cancel = true;
            Settings.BrowserHistory.Pop(); //Pop da saltare perchè appena navigo lo mette nello stack
            ChooserWebBrowser.Navigate(Settings.BrowserHistory.Pop());
        }

        #region UrlTextBox

        private void ImageUrlTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ImageUrlTextBox.Text.Contains("#http://"))
                ImageUrlTextBox.Text = ImageUrlTextBox.Text.Remove(0, ImageUrlTextBox.Text.IndexOf("http://"));
            if (ImageUrlTextBox.Text.StartsWith("http://"))
                ImageUrlTextBox.Select(7, ImageUrlTextBox.Text.Length - 6);
        }

        private void ImageUrlTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            Uri u;
            if (e.Key == Key.Enter && Uri.TryCreate(ImageUrlTextBox.Text, UriKind.Absolute, out u))
                ChooserWebBrowser.Navigate(new Uri(ImageUrlTextBox.Text));
        }
        
        #endregion
        
        #region WebBrowser

        private void ChooserWebBrowser_Navigating(object sender, NavigatingEventArgs e)
        {
            LoadingProgress.Visibility = Visibility.Visible;
        }

        private void ChooserWebBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            ImageUrlTextBox.Text = ChooserWebBrowser.Source.ToString();
            Settings.BrowserHistory.Push(ChooserWebBrowser.Source);
            LoadingProgress.Visibility = Visibility.Collapsed;
        }
        
        #endregion

        #region ApplicationBar Buttons

        private void ShowPicsApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            ExtractImagesHelper.ExtractImages(ChooserWebBrowser.Source, ChooserWebBrowser.SaveToString());
            NavigationService.Navigate(new Uri("/ImageListPage.xaml", UriKind.Relative));
        }

        private void GoHomeAppBarButton_Click(object sender, EventArgs e)
        {
            ChooserWebBrowser.Navigate(new Uri("http://www.google.com/m/?site=images"));
        }

        #endregion

        private void AboutApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

    }
}