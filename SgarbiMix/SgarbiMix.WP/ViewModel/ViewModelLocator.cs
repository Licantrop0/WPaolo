using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using WPCommon.Helpers;

namespace SgarbiMix.WP.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<INavigationService, NavigationService>();

            SimpleIoc.Default.Register<MainViewModel>();
        }

        public MainViewModel MainVM => ServiceLocator.Current.GetInstance<MainViewModel>();

        public UpdateViewModel UpdateVM => new UpdateViewModel(SimpleIoc.Default.GetInstance<INavigationService>());

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}