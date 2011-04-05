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

namespace NascondiChiappe
{
    public partial class ViewPhotosPage : PhoneApplicationPage
    {
        public ViewPhotosPage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (NavigationContext.QueryString.ContainsKey("Album"))
            {
                var AlbumId = Convert.ToInt32(NavigationContext.QueryString["Album"]);
                ImagePivot.ItemsSource = Settings.Albums[AlbumId].Images;
                var PhotoId = Convert.ToInt32(NavigationContext.QueryString["Photo"]);
                ImagePivot.SelectedIndex = PhotoId;
            }
        }
    }
}