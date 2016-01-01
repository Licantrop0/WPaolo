using TouchColors.ViewModel;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TouchColors.View
{
    public sealed partial class MainPage : Page
    {
        MainViewModel ViewModel => (MainViewModel)this.DataContext;

        public ElementTheme CurrentTheme
        {
            get
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("CurrentTheme"))
                    return (ElementTheme)ApplicationData.Current.LocalSettings.Values["CurrentTheme"];

                return this.RequestedTheme;
            }
            set
            {
                ApplicationData.Current.LocalSettings.Values["CurrentTheme"] = (int)value;
                this.RequestedTheme = value;
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
            this.RequestedTheme = CurrentTheme;

            ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseVisible);

        }

        private void SwitchTheme_Click(object sender, RoutedEventArgs e)
        {
            CurrentTheme = CurrentTheme == ElementTheme.Dark ?
                ElementTheme.Light : ElementTheme.Dark;
        }

        private void Questions_Click(object sender, RoutedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(QuestionsPage));
        }
    }
}