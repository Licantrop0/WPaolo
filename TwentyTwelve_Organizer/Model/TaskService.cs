using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Xml.Linq;

namespace TwentyTwelve_Organizer.Model
{
    public static class TaskService
    {
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