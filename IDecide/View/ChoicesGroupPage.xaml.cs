using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using IDecide.Localization;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace IDecide
{
    public partial class ChoicesGroupPage : PhoneApplicationPage
    {
        public ChoicesGroupPage()
        {
            InitializeComponent();
            CreateAppBar();
        }

        private void RemoveButton_Click(object sender, MouseButtonEventArgs e)
        {
        }

        private void EditChoicesButton_Click(object sender, RoutedEventArgs e)
        {
            var GroupName = ((Button)sender).DataContext as string;
            if (GroupName != null)
                NavigationService.Navigate(new Uri("/View/AddEditChoicesPage.xaml" + GroupName, UriKind.Relative));
        }

        private void CreateAppBar()
        {
            var AddChoiceGroupAppBarButton = new ApplicationBarIconButton();
            AddChoiceGroupAppBarButton.IconUri = new Uri("Toolkit.Content\\add_white.png", UriKind.Relative);
            AddChoiceGroupAppBarButton.Text = AppResources.AddGroup;
            AddChoiceGroupAppBarButton.Click += (sender, e) =>
            {
                NavigationService.Navigate(new Uri("/View/AddEditChoicesPage.xaml", UriKind.Relative));
            };
            ApplicationBar.Buttons.Add(AddChoiceGroupAppBarButton);
        }

    }
}