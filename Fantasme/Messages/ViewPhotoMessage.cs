using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using NascondiChiappe.Model;

namespace NascondiChiappe.Messages
{
    public class ViewPhotoMessage
    {
        public Photo SelectedPhoto { get; private set; }
        public string DirectoryName { get; private set; }
        public ViewPhotoMessage(Photo selectedPhoto, string directoryName)
        {
            SelectedPhoto = selectedPhoto;
            DirectoryName = directoryName;
        }
    }
}
