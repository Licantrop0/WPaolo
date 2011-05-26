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
    public partial class ImageList : UserControl
    {
        public event SelectionChangedEventHandler SelectedChanged = delegate { };
        public event EventHandler<GestureEventArgs> DoubleTap = delegate { };

        public ImageList()
        {
            InitializeComponent();
        }

        private void ImagesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedChanged.Invoke(sender, e);
        }

        private void GestureListener_DoubleTap(object sender, GestureEventArgs e)
        {
            var photo = (Image)sender;
            sender = photo.DataContext;

            DoubleTap.Invoke(sender, e);
        }
    }
}
