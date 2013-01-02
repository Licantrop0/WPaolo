using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPCommon;
using WPCommon.Helpers;
using TwentyTwelve_Organizer.Model;
using TwentyTwelve_Organizer.Sound;

namespace TwentyTwelve_Organizer.View
{
    public partial class TasksPage : PhoneApplicationPage
    {
        public TasksPage()
        {    
            InitializeComponent();
        }

        private void AddTaskButton_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            SoundManager.ButtonDownSound.Play();
        }

        private void AddTaskButton_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            SoundManager.ButtonUpSound.Play();
        }
    }
}