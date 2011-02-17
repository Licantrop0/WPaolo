using System;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using System.Xml.Linq;
using Microsoft.Phone.Marketplace;
using System.Windows;

namespace TwentyTwelve_Organizer
{
    public static class Settings
    {
        public static SoundEffect TickSound;
        public static SoundEffect ButtonUpSound;
        public static SoundEffect ButtonDownSound;
        public static DateTime EndOfTheWorld { get { return new DateTime(2012, 12, 21); } }

        public static ObservableCollection<Task> Tasks
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("tasks_list"))
                    IsolatedStorageSettings.ApplicationSettings["tasks_list"] = GetDefaultTasks();
                return (ObservableCollection<Task>)IsolatedStorageSettings.ApplicationSettings["tasks_list"];
            }
            set
            {
                if (Tasks != value)
                    IsolatedStorageSettings.ApplicationSettings["tasks_list"] = value;
            }
        }

        private static ObservableCollection<Task> GetDefaultTasks()
        {
            var oc = new ObservableCollection<Task>();
            (from xe in XDocument.Load("DefaultTasks.xml").Descendants("Task")
             orderby xe.Attribute("Description").Value
             select new Task(xe.Attribute("Description").Value,
                 Task.ParseDifficulty(xe.Attribute("Difficulty").Value))
             ).ToList().ForEach(t => oc.Add(t));
            return oc;
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