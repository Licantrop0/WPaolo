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

namespace PayMe
{
    public class Attendance
    {
        public int Id { get { return StartTime.GetHashCode() ^ EndTime.GetHashCode(); } }
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }
        public TimeSpan PauseTimeSpan { get; private set; }
        public double Income { get; private set; }
        public string CurrencySymbol { get; private set; }
        public string Customer { get; set; }
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