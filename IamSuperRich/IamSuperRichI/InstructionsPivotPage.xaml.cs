using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace IamSuperRichI
{
    public partial class InstructionsPivotPage : PhoneApplicationPage
    {

        public InstructionsPivotPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Save the Pivot control's SelectedIndex in page state
            PhoneApplicationService.Current.State["pivot_index"] = MainPivot.SelectedIndex;
        }

        private void OnPivotControlLoaded(object sender, RoutedEventArgs e)
        {
            // Restore the Pivot control's SelectedIndex
            if (PhoneApplicationService.Current.State.ContainsKey("pivot_index"))
                MainPivot.SelectedIndex = (int)PhoneApplicationService.Current.State["pivot_index"];
        }

        void Image_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            try
            {
                new MarketplaceDetailTask()
                {
                    ContentIdentifier = button.Tag.ToString()
                }.Show();
            }
            catch (InvalidOperationException)
            { /*do nothing */ }
        }

        private void Youtube_Click(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { URL = "http://www.youtube.com/watch?v=BiklqLjr9lg" }.Show();
        }
    }
}