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
using NascondiChiappe.ViewModel;

namespace NascondiChiappe
{
    public partial class ImageList : UserControl
    {
        private AlbumViewModel _vM;
        public AlbumViewModel VM
        {
            get
            {
                if (_vM == null)
                    _vM = DataContext as AlbumViewModel;
                return _vM;
            }
        }

        public ImageList()
        {
            InitializeComponent();
        }

        private void GestureListener_DoubleTap(object sender, GestureEventArgs e)
        {
            //TODO replace with actual image index
            VM.ShowImage.Execute(0);
        }
    }
}
