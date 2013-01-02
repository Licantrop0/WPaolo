using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;
using System.Runtime.Serialization;

namespace TwentyTwelve_Organizer.Model
{
    //questo enum rappresenta i numeri di giorni necessari al completamento di un determinato task
    public enum TaskDifficulty
    {
        VerySimple = 1,
        Simple = 5,
        Normal = 15,
        Hard = 40,
        VeryHard = 100
    }


    [DataContract]
    public class Task
    {
        public int Id { get { return Description.GetHashCode() ^ IsCompleted.GetHashCode(); } }

        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public TaskDifficulty Difficulty { get; set; }
        [DataMember]
        public bool IsCompleted { get; set; }

        public Task() { }

        public Task(string description, TaskDifficulty difficulty)
        {
            Description = description;
            Difficulty = difficulty;
            IsCompleted = false;
        }
    }
}