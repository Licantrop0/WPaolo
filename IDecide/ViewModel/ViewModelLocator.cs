/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:IDecide.ViewModel" x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using NascondiChiappe.Helpers;

namespace IDecide.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        private static ChoicesGroupViewModel _choicesGroupVM;
        private static AddEditChoicesViewModel _addEditChoicesVM;

        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            _choicesGroupVM = new ChoicesGroupViewModel();
            _addEditChoicesVM = new AddEditChoicesViewModel();

            ChoicesGroupVM.NavigationService = new NavigationService();
            AddEditChoicesVM.NavigationService = new NavigationService();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ChoicesGroupViewModel ChoicesGroupVM
        {
            get { return _choicesGroupVM; }
        }

        public AddEditChoicesViewModel AddEditChoicesVM
        {
            get { return _addEditChoicesVM; }
        }
    }
}