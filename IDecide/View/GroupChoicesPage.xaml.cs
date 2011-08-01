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
    public partial class GroupChoicesPage : PhoneApplicationPage
    {
        public GroupChoicesPage()
        {
            InitializeComponent();
            CreateAppBar();
            //GroupChoicesListBox.ItemsSource = Settings.ChoicesGroup.Select(c => c.Key).Distinct();
        }

        private void RemoveButton_Click(object sender, MouseButtonEventArgs e)
        {
            var GroupName = ((Rectangle)sender).DataContext as string;

            //Settings.ChoicesGroup.Where(i => i.Key == GroupName)
            //    .ForEach(i => Settings.ChoicesGroup.Remove(i));
        }

        private void EditChoicesButton_Click(object sender, RoutedEventArgs e)
        {
            var GroupName = ((Button)sender).DataContext as string;
            if (GroupName != null)
                NavigationService.Navigate(new Uri("/AddEditChoicesPage.xaml?key=" + GroupName, UriKind.Relative));
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
          //  Settings.SelectedGroup = ((RadioButton)sender).DataContext as string;
        }

        private void CreateAppBar()
        {
            ApplicationBar = new ApplicationBar();
            var AddChoiceGroupAppBarButton = new ApplicationBarIconButton();
            AddChoiceGroupAppBarButton.IconUri = new Uri("Toolkit.Content\\add_white.png", UriKind.Relative);
            AddChoiceGroupAppBarButton.Text = AppResources.AddGroup;
            AddChoiceGroupAppBarButton.Click += (sender, e) =>
            {
                NavigationService.Navigate(new Uri("/AddEditChoicesPage.xaml?key=test", UriKind.Relative));
            };
            ApplicationBar.Buttons.Add(AddChoiceGroupAppBarButton);
        }

    }
}