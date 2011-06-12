﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using System.IO.IsolatedStorage;
using System.IO;
using System.Xml.Linq;
using System.Collections.Generic;

namespace NascondiChiappe
{
    public partial class ViewPhotosPage : PhoneApplicationPage
    {
        public ViewPhotosPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (!AppContext.IsPasswordInserted)
            {
                NavigationService.Navigate(new Uri("/PasswordPage.xaml", UriKind.Relative));
                return;
            }

            if (!NavigationContext.QueryString.ContainsKey("Photo"))
            {
                NavigationService.GoBack();
                return;
            }

            CreateHtml();
        }

        private void CreateHtml()
        {
            var PhotoId = Convert.ToInt32(NavigationContext.QueryString["Photo"]);
            var CurrentPhoto = AppContext.CurrentAlbum.Photos[PhotoId];

            Wb.Base = AppContext.CurrentAlbum.DirectoryName;

            var html = new XDocument(
                new XElement("html",
                    new XElement("head",
                        new XElement("meta",
                            new XAttribute("name", "viewport"),
                            new XAttribute("content", "width=480,height=800")),
                        new XElement("body",
                            new XAttribute("style", "background-color:black"),
                            new XElement("img",
                                new XAttribute("src", CurrentPhoto.Name),
                                new XAttribute("width", "480"),
                                new XAttribute("style", string.Format(
                                    "rotation:{0}deg;margin-top:auto; margin-bottom:auto;",
                                    CurrentPhoto.RotationAngle))
                                            )))));


            IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();
            using (var isfs = isf.OpenFile(Wb.Base + "\\image.html", FileMode.Create))
            {
                using (var sw = new StreamWriter(isfs))
                {
                    sw.Write(html);
                    sw.Close();
                    isfs.Close();
                }
            }
        }

        private void Wb_Loaded(object sender, RoutedEventArgs e)
        {
            Wb.Navigate(new Uri("image.html", UriKind.Relative));
        }


        //private void ImagePivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (CurrentTransform == null)
        //        return;

        //    ResetPositions();
        //}

        //private void GestureListener_PinchStarted(object sender, PinchStartedGestureEventArgs e)
        //{
        //    CurrentImage = sender as Image;
        //    CurrentTransform = CurrentImage.RenderTransform as CompositeTransform;

        //    //Point firstTouch = e.GetPosition(CurrentImage, 0);
        //    //Point secondTouch = e.GetPosition(CurrentImage, 1);

        //    //Point center = new Point(firstTouch.X + (secondTouch.X - firstTouch.X) / 2.0,
        //    //                            firstTouch.Y + (secondTouch.Y - firstTouch.Y) / 2.0);

        //    //CurrentTransform.CenterX = center.X;
        //    //CurrentTransform.CenterY = center.Y;

        //    // Record the current scaling and rotation values
        //    _cx = CurrentTransform.ScaleX;
        //    _cy = CurrentTransform.ScaleY;
        //}

        //private void GestureListener_DragStarted(object sender, DragStartedGestureEventArgs e)
        //{
        //    CurrentImage = sender as Image;
        //    CurrentTransform = CurrentImage.RenderTransform as CompositeTransform;
        //}

        //private void GestureListener_DragDelta(object sender, DragDeltaGestureEventArgs e)
        //{
        //    //TODO: Implementare boundaries a seconda dello ScaleTransform
        //    if (CurrentTransform.ScaleX <= 1 && CurrentTransform.ScaleY <= 1)
        //        return;

        //    //var CenterWidth = CurrentImage.ActualWidth * CurrentTransform.ScaleX / 2;
        //    //var CenterHeight = CurrentImage.ActualHeight * CurrentTransform.ScaleY / 2;

        //    CurrentTransform.TranslateX += e.HorizontalChange;
        //    CurrentTransform.TranslateY += e.VerticalChange;
        //}

        //private void GestureListener_PinchDelta(object sender, PinchGestureEventArgs e)
        //{
        //    double cx = _cx * e.DistanceRatio;
        //    double cy = _cy * e.DistanceRatio;

        //    //If they're between 1.0 and 4.0, inclusive, apply them
        //    if (cx >= 1.0 && cx < 4.0 && cy >= 1.0 && cy < 4.0)
        //    {
        //        CurrentTransform.ScaleX = cx;
        //        CurrentTransform.ScaleY = cy;
        //    }
        //}

        //private void GestureListener_DoubleTap(object sender, GestureEventArgs e)
        //{
        //    CurrentImage = sender as Image;
        //    CurrentTransform = CurrentImage.RenderTransform as CompositeTransform;

        //    double cx = CurrentTransform.ScaleX * 1.5;
        //    double cy = CurrentTransform.ScaleY * 1.5;

        //    //If they're between 1.0 and 4.0, apply them
        //    if (cx >= 1.0 && cx < 4.0 && cy >= 1.0 && cy < 4.0)
        //    {
        //        //TODO: Implementare centro sul tap
        //        //var Center = e.GetPosition(CurrentImage);
        //        //CurrentTransform.CenterX = Center.X;
        //        //CurrentTransform.CenterY = Center.Y;
        //        CurrentTransform.ScaleX = cx;
        //        CurrentTransform.ScaleY = cy;
        //    }
        //    else
        //    {
        //        ResetPositions();
        //    }
        //}

        //private void ResetPositions()
        //{
        //    CurrentTransform.ScaleX = 1;
        //    CurrentTransform.ScaleY = 1;
        //    CurrentTransform.TranslateX = 0;
        //    CurrentTransform.TranslateY = 0;
        //}
    }
}