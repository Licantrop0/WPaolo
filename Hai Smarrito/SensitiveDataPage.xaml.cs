using System;
using System.Windows.Controls;
using Microsoft.Phone.Controls;

namespace NientePanico
{
    public partial class SensitiveDataPage : PhoneApplicationPage
    {
        public SensitiveDataPage()
        {
            InitializeComponent();
        }

        private void CardsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CardsListBox.SelectedIndex == -1)
                return;

            // Navigate to the new page
            NavigationService.Navigate(new Uri("/CardDataPage.xaml?selectedItem="
                + CardsListBox.SelectedIndex, UriKind.Relative));

            // Reset selected index to -1 (no selection)
            CardsListBox.SelectedIndex = -1;
        }

    }
}