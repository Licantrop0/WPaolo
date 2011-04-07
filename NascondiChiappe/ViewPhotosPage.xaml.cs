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
using System.Windows.Media.Imaging;

namespace NascondiChiappe
{
    public partial class ViewPhotosPage : PhoneApplicationPage
    {
        private double _cx, _cy;

        public ViewPhotosPage()
        {
            InitializeComponent();
        }

        CompositeTransform CurrentTransform;

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (NavigationContext.QueryString.ContainsKey("Album"))
            {
                var AlbumId = Convert.ToInt32(NavigationContext.QueryString["Album"]);
                ImagePivot.ItemsSource = Settings.Albums[AlbumId].Images;
                var PhotoId = Convert.ToInt32(NavigationContext.QueryString["Photo"]);
                if (PhotoId == -1) PhotoId = 0; //correzione se nessuna immagine era selezionata
                ImagePivot.SelectedIndex = PhotoId;
            }
        }

        private void ImagePivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentTransform == null)
                return;

            CurrentTransform.ScaleX = 1;
            CurrentTransform.ScaleY = 1;
            CurrentTransform.TranslateX = 0;
            CurrentTransform.TranslateY = 0;
        }

        private void GestureListener_PinchStarted(object sender, PinchStartedGestureEventArgs e)
        {
            var CurrentImage = sender as Image;
            CurrentTransform = CurrentImage.RenderTransform as CompositeTransform;
            
            // Record the current scaling and rotation values
            _cx = CurrentTransform.ScaleX;
            _cy = CurrentTransform.ScaleY;
        }

        private void GestureListener_DragStarted(object sender, DragStartedGestureEventArgs e)
        {
            var CurrentImage = sender as Image;
            CurrentTransform = CurrentImage.RenderTransform as CompositeTransform;
        }

       private void GestureListener_DragDelta(object sender, DragDeltaGestureEventArgs e)
        {
           //TODO: Implementare boundaries a seconda dello ScaleTransform
            if (CurrentTransform.ScaleX > 1 && CurrentTransform.ScaleY > 1)
            {
                CurrentTransform.TranslateX += e.HorizontalChange;
                CurrentTransform.TranslateY += e.VerticalChange;
            }
        }

        private void GestureListener_PinchDelta(object sender, PinchGestureEventArgs e)
        {
            double cx = _cx * e.DistanceRatio;
            double cy = _cy * e.DistanceRatio;

            //If they're between 1.0 and 4.0, inclusive, apply them
            if (cx >= 1.0 && cx <= 4.0 && cy >= 1.0 && cy <= 4.0)
            {
                CurrentTransform.ScaleX = cx;
                CurrentTransform.ScaleY = cy;
            }
        }


    }
}