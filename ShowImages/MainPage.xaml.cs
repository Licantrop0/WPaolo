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
using HtmlAgilityPack;
using System.Windows.Media.Imaging;

namespace ShowImages
{
    public partial class MainPage : PhoneApplicationPage
    {
        public List<string> ImageList = new List<string>();

        public MainPage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            ChooserWebBrowser.Navigate(Settings.BrowserHistory.Pop());
        }

        void ExtractImages(string htmlString)
        {
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(htmlString);

            var ImgLinks = new List<string>();
            foreach (var img in html.DocumentNode.Descendants("img"))
            {
                var src = img.GetAttributeValue("src", string.Empty);

                if (src.Contains("::")) //Google Images
                {
                    src = src.Substring(src.IndexOf("::") + 2);
                    int AndIndex = src.IndexOf("&");
                    src = "http://" + src.Remove(AndIndex == -1 ? src.Length - 1 : AndIndex);
                }

                if (src.EndsWith(".jpg", StringComparison.InvariantCultureIgnoreCase))
                    ImgLinks.Add(src);
            }

            ImageList = (from anchor in html.DocumentNode.Descendants("a")
                         let href = anchor.GetAttributeValue("href", string.Empty)
                         where href.EndsWith(".jpg", StringComparison.InvariantCultureIgnoreCase)
                         select href.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) ?
                         href : "http://" + ChooserWebBrowser.Source.Host + ChooserWebBrowser.Source.AbsolutePath + href)
                         .Union(ImgLinks)
                        .ToList();

            if (ImageList.Count == 0)
                ImageStackPanel.Children.Add(new TextBlock()
                {
                    Text = "No images found, or images are in an incompatible format.\nPlease press back to change the site.",
                    Margin = new Thickness(12),
                    TextWrapping = TextWrapping.Wrap,
                    FontSize = (double)Application.Current.Resources["PhoneFontSizeExtraLarge"]
                });
            else
                ImageList.ForEach(i => ImageStackPanel.Children.Add(
                    new Image() { Source = new BitmapImage(new Uri(i)) }));
        }

        private void ChooserWebBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            ImageUrlTextBox.Text = ChooserWebBrowser.Source.ToString();
            Settings.BrowserHistory.Push(ChooserWebBrowser.Source);
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ChooserWebBrowser.Visibility == Visibility.Collapsed)
            {
                e.Cancel = true;
                SetBrowserVisible(true);
            }
            else //Gestione del Back-Stack del Browser
            {
                if (Settings.BrowserHistory.IsEmpty) return;
                e.Cancel = true;
                Settings.BrowserHistory.Pop(); //Pop da saltare perchè appena navigo lo mette nello stack
                ChooserWebBrowser.Navigate(Settings.BrowserHistory.Pop());
            }
        }

        void SetBrowserVisible(bool visibility)
        {
            Settings.BrowserVisibility = visibility;
            ApplicationBar.Buttons.Clear();

            if (visibility) //Browser Visible
            {
                ApplicationBar.Buttons.Add(CreateShowPicsAppBarButton());
                ImageScroller.Visibility = Visibility.Collapsed;
                ChooserWebBrowser.Visibility = Visibility.Visible;
                ImageUrlTextBox.Visibility = Visibility.Visible;
            }
            else //Lista Immagini Visibile
            {
                ApplicationBar.Buttons.Add(CreateSaveAppBarButton());
                ImageScroller.Visibility = Visibility.Visible;
                ChooserWebBrowser.Visibility = Visibility.Collapsed;
                ImageUrlTextBox.Visibility = Visibility.Collapsed;
            }
        }


        #region ApplicationBar Buttons

        private ApplicationBarIconButton CreateSaveAppBarButton()
        {
            var SaveAppBarButton = new ApplicationBarIconButton();
            SaveAppBarButton.IconUri = new Uri("AppBar.Save.png", UriKind.Relative);
            SaveAppBarButton.Text = "Save";
            SaveAppBarButton.Click += delegate(object sender, EventArgs e)
            { BL.SaveFiles(ImageList); };
            return SaveAppBarButton;
        }

        private ApplicationBarIconButton CreateShowPicsAppBarButton()
        {
            var ShowPicsAppBarButton = new ApplicationBarIconButton();
            ShowPicsAppBarButton.IconUri = new Uri("AppBar.Images.png", UriKind.Relative);
            ShowPicsAppBarButton.Text = "Show Pics";
            ShowPicsAppBarButton.Click += ShowPicsApplicationBarIconButton_Click;
            return ShowPicsAppBarButton;
        }

        private void ShowPicsApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            ImageStackPanel.Children.Clear();
            SetBrowserVisible(false);
            ExtractImages(ChooserWebBrowser.SaveToString());
        }

        #endregion

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

    }
}