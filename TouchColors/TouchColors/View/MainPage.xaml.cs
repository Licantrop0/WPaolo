using TouchColors.ViewModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TouchColors.View
{
    public sealed partial class MainPage : Page
    {
        MainViewModel ViewModel => (MainViewModel)this.DataContext;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void SwitchTheme_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.RequestedTheme = this.RequestedTheme == ElementTheme.Dark ?
                ElementTheme.Light : ElementTheme.Dark;

        }

        private void Questions_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(QuestionsPage));
        }
    }
}