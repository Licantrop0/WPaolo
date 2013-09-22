using System.Windows;
using Microsoft.Phone.Controls;

namespace DeathTimerz
{
    public partial class DisclaimerPage : PhoneApplicationPage
    {
        public DisclaimerPage()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}