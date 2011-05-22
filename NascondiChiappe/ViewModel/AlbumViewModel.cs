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
using System.ComponentModel;

namespace NascondiChiappe.ViewModel
{
    public class AlbumViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(object sender, string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(sender, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        public Album Model { get; private set; }

        public AlbumViewModel(Album model)
        {
            Model = model;
            Model.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "Photos")
                    OnPropertyChanged(this, "HintVisibility");
            };
        }

        public Visibility HintVisibility
        {
            get
            {
                return Model.Photos.Count == 0 ?
                    Visibility.Visible :
                    Visibility.Collapsed;
            }
        }

    }
}
