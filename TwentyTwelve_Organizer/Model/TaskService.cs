using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace TwentyTwelve_Organizer.Model
{
    public static class TaskService
    {
        static readonly Uri _xapUrl = new Uri("TwentyTwelve_Organizer;component/Data/DefaultTasks.xml", UriKind.Relative);
        static XmlSerializer _xml { get { return new XmlSerializer(typeof(Task[])); } }
        static readonly IsolatedStorageFile _storage = IsolatedStorageFile.GetUserStoreForApplication();


        public static IEnumerable<Task>  LoadTasks()
        {
            if (!IsolatedStorageSettings.ApplicationSettings.Contains("tasks_list"))
                IsolatedStorageSettings.ApplicationSettings["tasks_list"] = GetDefaultTasks();
            return (IEnumerable<Task>)IsolatedStorageSettings.ApplicationSettings["tasks_list"];
        }

        public static void SaveTasks(IEnumerable<Task> tasks)
        {
            IsolatedStorageSettings.ApplicationSettings["tasks_list"] = tasks;
        }

        private static IEnumerable<Task> GetDefaultTasks()
        {
            return XDocument.Load("Data/DefaultTasks.xml").Descendants("Task")
                .OrderBy(xe => xe.Attribute("Description").Value)
                .Select(xe => new Task(xe.Attribute("Description").Value,
                    ParseDifficulty(xe.Attribute("Difficulty").Value)));
        }

        public static IEnumerable<Task> Load()
        {
            if (_storage.FileExists("Tasks.xml"))
            {
                using (var stream = _storage.OpenFile("Tasks.xml", FileMode.Open))
                {
                    return _xml.Deserialize(stream) as Task[];
                }
            }
            return GetNew();
        }

        public static void Save(IEnumerable<Task> tasks)
        {
            using (var stream = _storage.CreateFile("Tasks.xml"))
            {
                _xml.Serialize(stream, tasks);
            }
        }

        public static IEnumerable<Task> GetNew()
        {
            using (var stream = Application.GetResourceStream(_xapUrl).Stream)
            {
                return _xml.Deserialize(stream) as Task[];
            }
        }

        private static TaskDifficulty ParseDifficulty(string value)
        {
            switch (value)
            {
                case "1":
                    return TaskDifficulty.VerySimple;
                case "2":
                    return TaskDifficulty.Simple;
                case "4":
                    return TaskDifficulty.Hard;
                case "5":
                    return TaskDifficulty.VeryHard;
                default: //case "3"
                    return TaskDifficulty.Normal;
            }
        }
    }
}