using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TwentyTwelve_Organizer
{
    public partial class AddEditTaskPage : PhoneApplicationPage
    {
        Task CurrentTask;

        public AddEditTaskPage()
        {
            InitializeComponent();

            if (Settings.LightThemeEnabled)
            {
                var isource = new BitmapImage(new Uri("Images/2012background-white.jpg", UriKind.Relative));
                LayoutRoot.Background = new ImageBrush() { ImageSource = isource };
            }
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
            TaskNameTextBox.SelectionStart = TaskNameTextBox.Text.Length;
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
            Settings.Tasks.Remove(CurrentTask);
            AddThisTask();
            NavigationService.GoBack();
        }

        private void TaskAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TaskNameTextBox.Text)) return;
            AddThisTask();
            NavigationService.GoBack();
        }

        private void AddThisTask()
        {
            TaskDifficulty diff;
            if (r_verySimple.IsChecked.Value) diff = TaskDifficulty.VerySimple;
            else if (r_Simple.IsChecked.Value) diff = TaskDifficulty.Simple;
            else if (r_Normal.IsChecked.Value) diff = TaskDifficulty.Normal;
            else if (r_Hard.IsChecked.Value) diff = TaskDifficulty.Hard;
            else diff = TaskDifficulty.VeryHard;

            Settings.Tasks.Add(new Task(TaskNameTextBox.Text, diff));
        }

        private void TaskNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) AddEditButton.Focus();
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