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
using System.Collections.ObjectModel;

namespace NascondiChiappe
{
    public class AppContext
    {
        public static bool IsPasswordInserted = false;

        private static ObservableCollection<Album> _albums;
        public static ObservableCollection<Album> Albums
        {
            get
            {
                if (_albums == null)
                    _albums = new ObservableCollection<Album>();
                return _albums;
            }
            set { _albums = value; }
        }

    }
}
