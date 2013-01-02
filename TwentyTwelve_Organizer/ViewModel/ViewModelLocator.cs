using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace TwentyTwelve_Organizer.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<TasksViewModel>();
            SimpleIoc.Default.Register<AddEditTaskViewModel>();
            ServiceLocator.Current.GetInstance<AddEditTaskViewModel>();
        }

        public TasksViewModel TasksVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<TasksViewModel>();
            }
        }

        public AddEditTaskViewModel AddEditTaskVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AddEditTaskViewModel>();
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}