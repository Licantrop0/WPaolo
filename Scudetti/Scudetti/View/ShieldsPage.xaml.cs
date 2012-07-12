using Microsoft.Phone.Controls;

namespace Scudetti.View
{
    public partial class ShieldsPage : PhoneApplicationPage
    {
        public ShieldsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            var levelIndex = int.Parse(NavigationContext.QueryString["level"]);
            LayoutRoot.DataContext = AppContext.Levels[levelIndex];
            base.OnNavigatedTo(e);
        }
    }
}