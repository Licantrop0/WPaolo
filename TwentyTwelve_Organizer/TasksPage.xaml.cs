using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPCommon;

namespace TwentyTwelve_Organizer
{
    public partial class TasksPage : PhoneApplicationPage
    {
        public TasksPage()
        {    
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            ToDoCVS.Source = Settings.Tasks;
            CompletedCVS.Source = Settings.Tasks;
        }

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (TrialManagement.IsTrialMode)
                NavigationService.Navigate(new Uri("/DemoInfoPage.xaml", UriKind.Relative));
            else
                NavigationService.Navigate(new Uri("/AddEditTaskPage.xaml", UriKind.Relative));
        }

        private void RemoveButton_Click(object sender, MouseButtonEventArgs e)
        {
            if (TrialManagement.IsTrialMode)
                NavigationService.Navigate(new Uri("/DemoInfoPage.xaml", UriKind.Relative));
            else
                if (MessageBox.Show("Do you want to delete this taks?", "Confirm", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    Task taskToDelete = ((Rectangle)sender).DataContext as Task;
                    Settings.Tasks.Remove(taskToDelete);
                }
        }

        private void EditTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (TrialManagement.IsTrialMode)
                NavigationService.Navigate(new Uri("/DemoInfoPage.xaml", UriKind.Relative));
            else
            {
                Task taskToEdit = ((Button)sender).DataContext as Task;
                NavigationService.Navigate(new Uri("/AddEditTaskPage.xaml?id=" + taskToEdit.Id, UriKind.Relative));
            }
        }

        private void ToDo_Filter(object sender, System.Windows.Data.FilterEventArgs e)
        {
            if (e.Item == null)
                e.Accepted = false;
            else
                e.Accepted = !((Task)e.Item).IsCompleted;
        }

        private void Completed_Filter(object sender, System.Windows.Data.FilterEventArgs e)
        {
            if (e.Item == null)
                e.Accepted = false;
            else
                e.Accepted = ((Task)e.Item).IsCompleted;
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