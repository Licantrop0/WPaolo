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

namespace PayMe
{
    [DataContract]
    public class Attendance
    {
        public int Id { get { return StartTime.GetHashCode() ^ EndTime.GetHashCode(); } }

        [DataMember]
        public DateTime StartTime { get; private set; }
        [DataMember]
        public DateTime EndTime { get; private set; }
        [DataMember]
        public TimeSpan PauseTimeSpan { get; private set; }
        [DataMember]
        public double Income { get; private set; }
        [DataMember]
        public string CurrencySymbol { get; private set; }
        [DataMember]
        public string CustomerName { get; set; }
        [DataMember]
        public string Description { get; set; }

        public Attendance(DateTime startTime, DateTime endTime, TimeSpan pauseTimeSpan, double income, string currencySymbol)
        {
            StartTime = startTime;
            EndTime = endTime;
            PauseTimeSpan = pauseTimeSpan;
            Income = income;
            CurrencySymbol = currencySymbol;
        }
    }
}