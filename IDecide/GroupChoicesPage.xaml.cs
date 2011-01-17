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

namespace IDecide
{
    public partial class GroupChoicesPage : PhoneApplicationPage
    {
        public GroupChoicesPage()
        {
            InitializeComponent();
            GroupChoicesListBox.ItemsSource = Settings.ChoicesGroup.Select(c => c.Key).Distinct();
        }

        private void RemoveButton_Click(object sender, MouseButtonEventArgs e)
        {

        }

        private void EditChoicesButton_Click(object sender, RoutedEventArgs e)
        {
            var GroupName = ((Button)sender).DataContext as string;
            if (GroupName != null)
                NavigationService.Navigate(new Uri("/EditChoicesPage.xaml?key=" + GroupName, UriKind.Relative));
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Settings.SelectedGroup = ((RadioButton)sender).DataContext as string;
        }
    }

    public class ResourceTranslator : IValueConverter
    {

        #region IValueConverter Members

        //Resource Translator
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return AppResources.ResourceManager.GetString((string)value);
        }

        // ConvertBack is not implemented for a OneWay binding.
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }



}