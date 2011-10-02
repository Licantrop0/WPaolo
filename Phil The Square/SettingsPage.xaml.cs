using FillTheSquare.ViewModel;
using Microsoft.Phone.Controls;

namespace FillTheSquare
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            LayoutRoot.DataContext = SettingsViewModel.Instance;
        }
    }
}