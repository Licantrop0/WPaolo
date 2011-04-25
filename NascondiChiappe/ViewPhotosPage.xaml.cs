using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Controls;

namespace NascondiChiappe
{
    public partial class ViewPhotosPage : PhoneApplicationPage
    {
        private double _cx, _cy;
        private Image CurrentImage;
        private CompositeTransform CurrentTransform;

        public ViewPhotosPage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!Settings.IsPasswordInserted)
            {
                NavigationService.Navigate(new Uri("/PasswordPage.xaml", UriKind.Relative));
                return;
            }

            if (NavigationContext.QueryString.ContainsKey("Album"))
            {
                var AlbumId = NavigationContext.QueryString["Album"];
                ImagePivot.ItemsSource = Settings.Albums.First(a => a.DirectoryName == AlbumId).Photos;
                var PhotoId = Convert.ToInt32(NavigationContext.QueryString["Photo"]);
                if (PhotoId == -1) PhotoId = 0; //correzione se nessuna immagine era selezionata
                ImagePivot.SelectedIndex = PhotoId;
            }
        }

        private void ImagePivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentTransform == null)
                return;

            ResetPositions();
        }

        private void GestureListener_PinchStarted(object sender, PinchStartedGestureEventArgs e)
        {
            CurrentImage = sender as Image;
            CurrentTransform = CurrentImage.RenderTransform as CompositeTransform;

            // Record the current scaling and rotation values
            _cx = CurrentTransform.ScaleX;
            _cy = CurrentTransform.ScaleY;
        }

        private void GestureListener_DragStarted(object sender, DragStartedGestureEventArgs e)
        {
            CurrentImage = sender as Image;
            CurrentTransform = CurrentImage.RenderTransform as CompositeTransform;
        }

        private void GestureListener_DragDelta(object sender, DragDeltaGestureEventArgs e)
        {
            //TODO: Implementare boundaries a seconda dello ScaleTransform
            if (CurrentTransform.ScaleX <= 1 && CurrentTransform.ScaleY <= 1)
                return;

            //var CenterWidth = CurrentImage.ActualWidth * CurrentTransform.ScaleX / 2;
            //var CenterHeight = CurrentImage.ActualHeight * CurrentTransform.ScaleY / 2;

            CurrentTransform.TranslateX += e.HorizontalChange;
            CurrentTransform.TranslateY += e.VerticalChange;
        }

        private void GestureListener_PinchDelta(object sender, PinchGestureEventArgs e)
        {
            double cx = _cx * e.DistanceRatio;
            double cy = _cy * e.DistanceRatio;

            //If they're between 1.0 and 4.0, inclusive, apply them
            if (cx >= 1.0 && cx < 4.0 && cy >= 1.0 && cy < 4.0)
            {
                CurrentTransform.ScaleX = cx;
                CurrentTransform.ScaleY = cy;
            }
        }

        private void GestureListener_DoubleTap(object sender, GestureEventArgs e)
        {
            CurrentImage = sender as Image;
            CurrentTransform = CurrentImage.RenderTransform as CompositeTransform;

            double cx = CurrentTransform.ScaleX * 1.5;
            double cy = CurrentTransform.ScaleY * 1.5;

            //If they're between 1.0 and 4.0, apply them
            if (cx >= 1.0 && cx < 4.0 && cy >= 1.0 && cy < 4.0)
            {
                //TODO: Implementare centro sul tap
                //var Center = e.GetPosition(this);
                //CurrentTransform.CenterX = Center.X;
                //CurrentTransform.CenterY = Center.Y;
                CurrentTransform.ScaleX = cx;
                CurrentTransform.ScaleY = cy;
            }
            else
            {
                ResetPositions();
            }
        }

        private void ResetPositions()
        {
            CurrentTransform.ScaleX = 1;
            CurrentTransform.ScaleY = 1;
            CurrentTransform.TranslateX = 0;
            CurrentTransform.TranslateY = 0;
        }
    }
}