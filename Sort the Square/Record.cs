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
using System.Runtime.Serialization;

namespace SortTheSquare
{
    [DataContract]
    public class Record
    {
        public int Id { get { return Date.GetHashCode(); } }

        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Size { get; set; }
        [DataMember]
        public DateTime Date { get; set; }
        [DataMember]
        public TimeSpan ElapsedTime { get; set; }

        public Record(int size, DateTime date, TimeSpan elapsedTime)
        {
            Size = size + "x" + size;
            Date = date;
            ElapsedTime = elapsedTime;
            Name = string.Empty;
        }
    }
}