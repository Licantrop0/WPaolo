/*
In App.xaml:
<Application.Resources>
<vm:ViewModelLocator xmlns:vm="clr-namespace:TouchColors"
x:Key="Locator" />
</Application.Resources>
In the View:
DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
You can also use Blend to do all this with the tool's support.
See http://www.galasoft.ch/mvvm
*/
using Microsoft.Practices.Unity;
using TouchColors.Helper;


namespace TouchColors.ViewModel
{

    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        private UnityContainer _ioc;

        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
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

            //if (ViewModelBase.IsInDesignModeStatic)
            //{
            //    SimpleIoc.Default.Register<ISpeechHelper, DesignSpeechHelper>();
            //}
            //else
            //{
            //}
            _ioc = new UnityContainer();
            _ioc.RegisterType<ISpeechHelper, SpeechHelper>();

        }

        public MainViewModel MainVM
        {
            get
            {
                return _ioc.Resolve<MainViewModel>();
            }
        }

        public QuestionsViewModel QuestionsVM
        {
            get
            {
                return _ioc.Resolve<QuestionsViewModel>();
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }

    }

}