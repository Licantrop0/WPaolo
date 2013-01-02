using System;
using System.Collections.Generic;
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

        private static IList<Task> _tasks;
        public static IList<Task> Tasks
        {
            get
            {
                if (_tasks == null)
                    _tasks = TaskService.LoadTasks().ToList();
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