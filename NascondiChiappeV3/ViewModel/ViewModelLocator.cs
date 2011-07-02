/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:NascondiChiappe"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using NascondiChiappe.Helpers;

namespace NascondiChiappe.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        private static AlbumsViewModel _albumsVM;
        private static AddRenameAlbumViewModel _addRenameAlbumVM;
        private static ViewPhotosViewModel _viewPhotosVM;

        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            _albumsVM = new AlbumsViewModel();
            _addRenameAlbumVM = new AddRenameAlbumViewModel();
            _viewPhotosVM = new ViewPhotosViewModel();

            AlbumsVM.NavigationService = new NavigationService();
            AddRenameAlbumVM.NavigationService = new NavigationService();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public AlbumsViewModel AlbumsVM
        {
            get { return _albumsVM; }
        }

        public AddRenameAlbumViewModel AddRenameAlbumVM
        {
            get { return _addRenameAlbumVM; }
        }

        public ViewPhotosViewModel ViewPhotosVM
        {
            get { return _viewPhotosVM; }
        }
    }
}