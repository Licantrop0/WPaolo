using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using TwentyTwelve_Organizer.Model;
using TwentyTwelve_Organizer.ViewModel;

namespace TwentyTwelve_Organizer
{
    public static class AppContext
    {
        public static DateTime EndOfTheWorld { get { return new DateTime(2012, 12, 21); } }

        private static ObservableCollection<TaskViewModel> _tasks;
        public static ObservableCollection<TaskViewModel> Tasks
        {
            get
            {
                if (_tasks == null)
                    _tasks = new ObservableCollection<TaskViewModel>(
                        TaskService.LoadTasks()
                        .Select(t => new TaskViewModel(t)));

                return _tasks;
            }
        }


        public static bool LightThemeEnabled
        {
            get
            {
                return (Visibility)Application.Current.Resources["PhoneLightThemeVisibility"] == Visibility.Visible;
            }
        }
    }
}