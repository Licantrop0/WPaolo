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

namespace FillTheSquare
{
    [DataContract]
    public class Record
    {
        [DataMember]
        public int seconds { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public DateTime date { get; set; }

        public Record(int secondsIn, string nameIn, DateTime dateIn)
        {
            seconds = secondsIn;
            name = nameIn;
            date = dateIn;
        }
    }
}
