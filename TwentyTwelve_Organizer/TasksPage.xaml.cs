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

namespace TwentyTwelve_Organizer
{
    public partial class TasksPage : PhoneApplicationPage
    {
        public TasksPage()
        {    
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            //il cvs è il CollectionViewSource impostato nello XAML,
            //serve per fare il sorting automatico dei task completati
            cvs.Source = Settings.Tasks;
        }

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.IsTrialMode)
            {
                NavigationService.Navigate(new Uri("/DemoInfoPage.xaml", UriKind.Relative));
            }
            else
                NavigationService.Navigate(new Uri("/AddEditTaskPage.xaml", UriKind.Relative));
        }

        private void RemoveButton_Click(object sender, MouseButtonEventArgs e)
        {
            if (Settings.IsTrialMode)
            {
                NavigationService.Navigate(new Uri("/DemoInfoPage.xaml", UriKind.Relative));
            }
            else
                if (MessageBox.Show("Do you want to delete this taks?", "Confirm", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                Task taskToDelete = ((Rectangle)sender).DataContext as Task;
                Settings.Tasks.Remove(taskToDelete);
            }
        }

        private void EditTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.IsTrialMode)
            {
                NavigationService.Navigate(new Uri("/DemoInfoPage.xaml", UriKind.Relative));
            }
            else
            {
                Task taskToEdit = ((Button)sender).DataContext as Task;
                NavigationService.Navigate(new Uri("/AddEditTaskPage.xaml?id=" + taskToEdit.Id, UriKind.Relative));
            }
        }

        private void AddTaskButton_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            Settings.ButtonDownSound.Play();
        }

        private void AddTaskButton_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            Settings.ButtonUpSound.Play();
        }

    }
}