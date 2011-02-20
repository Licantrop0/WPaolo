using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Data;
using WPCommon;
using Microsoft.Phone.Shell;

namespace IDecide
{
    public partial class GroupChoicesPage : PhoneApplicationPage
    {
        public GroupChoicesPage()
        {
            InitializeComponent();
            CreateAppBar();
            GroupChoicesListBox.ItemsSource = Settings.ChoicesGroup.Select(c => c.Key).Distinct();
        }

        private void RemoveButton_Click(object sender, MouseButtonEventArgs e)
        {
            var GroupName = ((Rectangle)sender).DataContext as string;

            Settings.ChoicesGroup.Where(i => i.Key == GroupName)
                .ForEach(i => Settings.ChoicesGroup.Remove(i));
        }

        private void EditChoicesButton_Click(object sender, RoutedEventArgs e)
        {
            var GroupName = ((Button)sender).DataContext as string;
            if (GroupName != null)
                NavigationService.Navigate(new Uri("/AddEditChoicesPage.xaml?key=" + GroupName, UriKind.Relative));
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Settings.SelectedGroup = ((RadioButton)sender).DataContext as string;
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

    public class ResourceTranslatorConverter : IValueConverter
    {
        //Resource Translator
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var TranslatedString = AppResources.ResourceManager.GetString((string)value);
            return string.IsNullOrEmpty(TranslatedString) ? value : TranslatedString;
        }

        // ConvertBack is not implemented for a OneWay binding.
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AmISelectedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Settings.SelectedGroup == (string)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}