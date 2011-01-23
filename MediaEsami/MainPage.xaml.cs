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

namespace MediaEsami
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            List<Exam> Esami = new List<Exam>() { new Exam("mate 1", 6), new Exam("fisica 1", 6) };
            esamiListBox.ItemsSource = Esami;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Exam examToEdit = ((Button)sender).DataContext as Exam;
            NavigationService.Navigate(new Uri("/AddEditExamPage.xaml?id=" + examToEdit.Id, UriKind.Relative));
        }
    }
}