using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Media;
using System.Linq;

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
                LoadingProgress.Maximum = Settings.CurrentImageList.Count;
                ImageListBox.ItemsSource = Settings.CurrentImageList;
            }
            else
                NoImagesTextBlock.Visibility = Visibility.Visible;
        }

        private void SaveAllApplicationBarButton_Click(object sender, EventArgs e)
        {
            IsSaving = true;
            LoadingProgress.Value = LoadingProgress.Minimum;

            foreach (var u in Settings.CurrentImageList.Where(i => i.Value == true).Select(i => i.Key))
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
            if (LoadingProgress.Value > LoadingProgress.Maximum)
            {
                LoadingProgress.Value = LoadingProgress.Minimum;
                if (IsSaving)
                {
                    MessageBox.Show("Images saved in your Media Library");
                    IsSaving = false;
                }
            }
        }

        private void Image_ImageOpened(object sender, RoutedEventArgs e)
        {
            LoadingProgress.Value += 1;
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            if (e.ErrorException.Message == "AG_E_NETWORK_ERROR")
            {
                LoadingProgress.Maximum -= 1;
                LoadingProgress_ValueChanged(sender, null);
                var imageSource = ((BitmapImage)((Image)sender).Source).UriSource.ToString();
                var imgToDelete = Settings.CurrentImageList.Where(i=> i.Key == imageSource).Single();
                //Settings.CurrentImageList.Remove(imgToDelete);
            }
        }

    }
}