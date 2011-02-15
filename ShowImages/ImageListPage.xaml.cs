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
        bool AllOpened = false;
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

                ImagePivot.ItemsSource = Settings.CurrentImageList;
            }
            else
                NoImagesTextBlock.Visibility = Visibility.Visible;
        }

        private void SaveAllApplicationBarButton_Click(object sender, EventArgs e)
        {
            LoadingProgress.Value = LoadingProgress.Minimum;
            LoadingProgress.Maximum = Settings.CurrentImageList.Where(i => i.Value).Count();

            IsSaving = true;
            foreach (var u in Settings.CurrentImageList.Where(i => i.Value).Select(i => i.Key))
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
                    catch (InvalidOperationException) { DecrementProgress(); }
                    catch (ArgumentException) { DecrementProgress(); }

                };
                wc.OpenReadAsync(new Uri(u));
            }
        }

        private void LoadingProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (LoadingProgress.Value >= LoadingProgress.Maximum)
            {
                AllOpened = true;
                if (IsSaving)
                {
                    MessageBox.Show("Images saved in your Media Library");
                    IsSaving = false;
                }
                LoadingProgress.Value = LoadingProgress.Minimum;
            }
        }

        private void Image_ImageOpened(object sender, RoutedEventArgs e)
        {
            if (!AllOpened)
                LoadingProgress.Value += 1;
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            if (e.ErrorException.Message == "AG_E_NETWORK_ERROR")
            {
                DecrementProgress();
                var imageSource = ((BitmapImage)((Image)sender).Source).UriSource.ToString();
                var imgToDelete = Settings.CurrentImageList.Where(i => i.Key == imageSource).Single();
                Settings.CurrentImageList.Remove(imgToDelete);
            }
        }

        private void DecrementProgress()
        {
            LoadingProgress.Maximum -= 1;
            LoadingProgress_ValueChanged(null, null);
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

    }
}