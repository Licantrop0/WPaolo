using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TwentyTwelve_Organizer.Model;
using TwentyTwelve_Organizer.Sound;

namespace TwentyTwelve_Organizer.View
{
    public partial class AddEditTaskPage : PhoneApplicationPage
    {
        public AddEditTaskPage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            TaskNameTextBox.Focus();
            TaskNameTextBox.SelectionStart = TaskNameTextBox.Text.Length;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (NavigationContext.QueryString.ContainsKey("id"))
            {
                var id = Convert.ToInt32(NavigationContext.QueryString["id"]);
            }
            else
            {
            }
        }

        private void TaskNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                this.Focus();
        }

        private void AddEditButton_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            SoundManager.ButtonDownSound.Play();
        }

        private void AddEditButton_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            SoundManager.ButtonUpSound.Play();
        }

    }
}