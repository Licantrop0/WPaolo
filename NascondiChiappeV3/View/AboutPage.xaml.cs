using System.Reflection;
using Microsoft.Phone.Controls;
using NascondiChiappe.Localization;
using WPCommon.ViewModel;
using System.Windows.Media;
using System;
using System.Windows.Media.Imaging;
namespace NascondiChiappe.View
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();

            var name = Assembly.GetExecutingAssembly().FullName;
            var VM = new AboutViewModel()
            {
                AppName = AppResources.AppName,
                AppVersion = new AssemblyName(name).Version.ToString(),
                BackgroundStackPanel = new ImageBrush() { ImageSource = new BitmapImage(new Uri("SplashScreenImage.jpg", UriKind.Relative))},
                DefaultFont = new FontFamily("/NascondiChiappe;component/Fonts/Fonts.zip#Harlow Solid Italic"),
                MinFontSize = 34                
            };

            WPMEAbout.VM = VM;
        }
    }
}