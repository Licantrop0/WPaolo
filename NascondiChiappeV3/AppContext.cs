using System.Collections.ObjectModel;
using NascondiChiappe.Model;
using NascondiChiappe.ViewModel;

namespace NascondiChiappe
{
    public class AppContext
    {
        public static bool IsPasswordInserted = false;

        private static ObservableCollection<ImageListViewModel> _albums;
        public static ObservableCollection<ImageListViewModel> Albums
        {
            get
            {
                if (_albums == null)
                    _albums = new ObservableCollection<ImageListViewModel>();
                return _albums;
            }
        }
    }
}
