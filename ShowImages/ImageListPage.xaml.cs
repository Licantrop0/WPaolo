using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Media;
namespace ShowImages
{
    public partial class ImageListPage : PhoneApplicationPage
    {
        MediaLibrary library = new MediaLibrary();
        bool IsSaving = false;

        public ImageListPage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (Settings.CurrentImageList.Count == 0)
                ImageStackPanel.Children.Add(new TextBlock()
                {
                    Text = "No images found, or images are in an incompatible format.\nPlease press back to change the site.",
                    Margin = new Thickness(12),
                    TextWrapping = TextWrapping.Wrap,
                    FontSize = (double)Application.Current.Resources["PhoneFontSizeExtraLarge"]
                });
            else
            {
                LoadingProgress.IsIndeterminate = true;
                foreach (var ImageUrl in Settings.CurrentImageList)
                {
                    var i = new Image()
                    {
                        Source = new BitmapImage(new Uri(ImageUrl)),
                        Margin = new Thickness(0, 0, 0, 8)
                        
                    };

                    i.ImageOpened += delegate(object s, RoutedEventArgs evnt)
                    {
                        LoadingProgress.Value += 1;
                    };
                    ImageStackPanel.Children.Add(i);
                }
            }
        }

        private void SaveAllApplicationBarButton_Click(object sender, EventArgs e)
        {
            IsSaving = true;
            LoadingProgress.Value = LoadingProgress.Minimum;
            foreach (var u in Settings.CurrentImageList)
            {
                WebClient wc = new WebClient();
                wc.AllowReadStreamBuffering = true;
                wc.OpenReadCompleted += delegate(object s, OpenReadCompletedEventArgs evnt)
                {
                    LoadingProgress.Value += 1;
                    if (evnt.Cancelled || evnt.Error != null || evnt.Result == null) return;

                    try
                    {
                        library.SavePicture(Path.GetFileName(u), evnt.Result);
                    }
                    catch (InvalidOperationException) { }
                };
                wc.OpenReadAsync(new Uri(u));
            }
        }

        private void LoadingProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (LoadingProgress.Value >= LoadingProgress.Maximum)
            {
                LoadingProgress.Value = LoadingProgress.Minimum;
                if (IsSaving)
                {
                    MessageBox.Show("Images saved to your Media Library");
                    IsSaving = false;
                }
            }

        }

    }
}