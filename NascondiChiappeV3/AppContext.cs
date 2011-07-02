using System.Collections.ObjectModel;
using NascondiChiappe.Model;
using NascondiChiappe.ViewModel;

namespace NascondiChiappe
{
    public class AppContext
    {
        public static bool IsPasswordInserted = false;
        public static ObservableCollection<ImageListViewModel> Albums { get; set; }
    }
}
