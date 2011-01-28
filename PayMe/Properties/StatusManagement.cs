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
using System.IO.IsolatedStorage;

namespace PayMe
{
    public enum Status
    {
        Stopped,
        Started,
        Paused,
        Resumed
    }

    public static class StatusManagement
    {

        public static Status CurrentStatus
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("current_status"))
                    IsolatedStorageSettings.ApplicationSettings["current_status"] = Status.Stopped;
                return (Status)IsolatedStorageSettings.ApplicationSettings["current_status"];
            }
            set
            {
                if (CurrentStatus != value)
                    IsolatedStorageSettings.ApplicationSettings["current_status"] = value;
            }
        }

        public static DateTime StartTime
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("start_time"))
                    IsolatedStorageSettings.ApplicationSettings["start_time"] = DateTime.Now;
                return (DateTime)IsolatedStorageSettings.ApplicationSettings["start_time"];
            }
            set
            {
                if (StartTime != value)
                    IsolatedStorageSettings.ApplicationSettings["start_time"] = value;
            }
        }

        public static DateTime StartPauseTime
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("start_pause_time"))
                    IsolatedStorageSettings.ApplicationSettings["start_pause_time"] = DateTime.Now;
                return (DateTime)IsolatedStorageSettings.ApplicationSettings["start_pause_time"];
            }
            set
            {
                if (StartPauseTime != value)
                    IsolatedStorageSettings.ApplicationSettings["start_pause_time"] = value;
            }
        }

        public static TimeSpan PauseTimeSpan
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("pause_timespan"))
                    IsolatedStorageSettings.ApplicationSettings["pause_timespan"] = new TimeSpan();
                return (TimeSpan)IsolatedStorageSettings.ApplicationSettings["pause_timespan"];
            }
            set
            {
                if (PauseTimeSpan != value)
                    IsolatedStorageSettings.ApplicationSettings["pause_timespan"] = value;
            }
        }

    }
}
