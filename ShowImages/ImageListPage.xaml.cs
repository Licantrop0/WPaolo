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
        bool IsSaving = false;
        MediaLibrary library = new MediaLibrary();

        public ImageListPage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (Settings.CurrentImageList.Count > 0)
            {
                LoadingProgress.IsIndeterminate = true;
                LoadingProgress.Maximum = Settings.CurrentImageList.Count;

                foreach (var ImageUrl in Settings.CurrentImageList)
                {
                    var i = new Image() { Source = new BitmapImage(new Uri(ImageUrl)), Margin = new Thickness(0, 0, 0, 8) };
                    i.ImageOpened += delegate(object s, RoutedEventArgs evnt) { LoadingProgress.Value += 1; };

                    ImageStackPanel.Children.Add(i);
                }
            }
            else
                NoImagesTextBlock.Visibility = Visibility.Visible;
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
                    catch (ArgumentException) { }

                };
                wc.OpenReadAsync(new Uri(u));
            }
        }


        //    Queue<String> uris = new Queue<String>(Settings.CurrentImageList);
        //    SaveNextUri(uris);
        //}

        //private void SaveNextUri(Queue<String> uris)
        //{
        //    if (uris.Count == 0) return;
        //    string u = uris.Dequeue();
            
        //    WebClient wc = new WebClient();
        //    wc.AllowReadStreamBuffering = true;

        //    wc.OpenReadCompleted += delegate(object sender, OpenReadCompletedEventArgs e)
        //    {
        //        LoadingProgress.Value += 1;
        //        if (e.Cancelled || e.Error != null || e.Result == null) return;

        //        try
        //        {
        //            library.SavePicture(Path.GetFileNameWithoutExtension(u), e.Result);
        //        }
        //        catch (InvalidOperationException) { }
        //        catch (ArgumentException) { }
        //        SaveNextUri(uris);
        //    };

        //    wc.OpenReadAsync(new Uri(u));
        //}

        private void LoadingProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsSaving) LoadingProgress.IsIndeterminate = false;

            if (LoadingProgress.Value >= LoadingProgress.Maximum)
            {
                LoadingProgress.IsIndeterminate = false;
                LoadingProgress.Value = LoadingProgress.Minimum;
                if (IsSaving)
                {
                    MessageBox.Show("Images saved in your Media Library");
                    IsSaving = false;
                }
            }
        }

    }
}