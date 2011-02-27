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
        public string Size { get; set; }
        [DataMember]
        public DateTime CurrentDate { get; set; }
        [DataMember]
        public double ElapsedTime { get; set; }

        public Record(int size, DateTime currentDate, double elapsedTime)
        {
            Size = size + "x" + size;
            CurrentDate = currentDate;
            ElapsedTime = elapsedTime;
        }
    }
}
