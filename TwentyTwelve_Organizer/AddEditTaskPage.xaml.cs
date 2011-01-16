using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Phone.Controls;

namespace TwentyTwelve_Organizer
{
    public partial class AddEditTaskPage : PhoneApplicationPage
    {
        Task CurrentTask;

        public AddEditTaskPage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (NavigationContext.QueryString.ContainsKey("id"))
            {
                AddEditButton.Content = "Edit";
                AddEditButton.Click += TaskEdit_Click;
                PageTitle.Text = "Edit Task";
                CurrentTask = Settings.Tasks.Where(t =>
                    t.Id == Convert.ToInt32(NavigationContext.QueryString["id"])).First();
                visualizeTask();
            }
            else
            {
                AddEditButton.Content = "Add";
                AddEditButton.Click += TaskAdd_Click;
                PageTitle.Text = "Add Task";
            }

            TaskNameTextBox.Focus();
            TaskNameTextBox.SelectAll();
        }

        private void visualizeTask()
        {
            TaskNameTextBox.Text = CurrentTask.Description;

            switch (CurrentTask.Difficulty)
            {
                case TaskDifficulty.VerySimple:
                    r_verySimple.IsChecked = true;
                    break;
                case TaskDifficulty.Simple:
                    r_Simple.IsChecked = true;
                    break;
                case TaskDifficulty.Hard:
                    r_Hard.IsChecked = true;
                    break;
                case TaskDifficulty.VeryHard:
                    r_veryHard.IsChecked = true;
                    break;
                default: //Normal
                    r_Normal.IsChecked = true;
                    break;
            }
        }

        private void TaskEdit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TaskNameTextBox.Text)) return;

            CurrentTask.Description = TaskNameTextBox.Text;

            if (r_verySimple.IsChecked.Value) CurrentTask.Difficulty = TaskDifficulty.VerySimple;
            else if (r_Simple.IsChecked.Value) CurrentTask.Difficulty = TaskDifficulty.Simple;
            else if (r_Normal.IsChecked.Value) CurrentTask.Difficulty = TaskDifficulty.Normal;
            else if (r_Hard.IsChecked.Value) CurrentTask.Difficulty = TaskDifficulty.Hard;
            else CurrentTask.Difficulty = TaskDifficulty.VeryHard;

            NavigationService.Navigate(new Uri("/TasksPage.xaml", UriKind.Relative));
        }

        private void TaskAdd_Click(object sender, RoutedEventArgs e)
        {
            TaskDifficulty diff = TaskDifficulty.Normal;
            if (r_verySimple.IsChecked.Value) diff = TaskDifficulty.VerySimple;
            else if (r_Simple.IsChecked.Value) diff = TaskDifficulty.Simple;
            else if (r_Normal.IsChecked.Value) diff = TaskDifficulty.Normal;
            else if (r_Hard.IsChecked.Value) diff = TaskDifficulty.Hard;
            else diff = TaskDifficulty.VeryHard;

            Settings.Tasks.Add(new Task(TaskNameTextBox.Text, diff));
            NavigationService.Navigate(new Uri("/TasksPage.xaml", UriKind.Relative));
        }

        private void TaskNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddEditButton.Focus();
            }
        }

        private void AddEditButton_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            Settings.ButtonDownSound.Play();
        }

        private void AddEditButton_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            Settings.ButtonUpSound.Play();
        }

    }
}