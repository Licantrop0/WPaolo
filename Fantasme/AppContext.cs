using System.Collections.ObjectModel;
using NascondiChiappe.Model;
using NascondiChiappe.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace NascondiChiappe
{
    public class AppContext
    {
        public static bool IsPasswordInserted = false;
        public static IList<Photo> Photos { get; set; }

        private static ObservableCollection<AlbumViewModel> _albums;
        public static ObservableCollection<AlbumViewModel> Albums
        {
            get
            {
                if (_albums == null)
                {
                    var groups = from p in Photos
                                 group p by p.Album into g
                                 select new AlbumViewModel(g);
                                 
                    _albums = new ObservableCollection<AlbumViewModel>(groups);
                }
                return _albums;
            }
        }

    }
}
