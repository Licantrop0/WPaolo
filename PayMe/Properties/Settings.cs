using System;
using System.IO.IsolatedStorage;
using System.Globalization;

namespace PayMe
{
    public static class Settings
    {
        public static string CurrencySymbol { get { return CultureInfo.CurrentUICulture.NumberFormat.CurrencySymbol; } }
        public static Double? HourlyPayment
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("hourly_payment"))
                    IsolatedStorageSettings.ApplicationSettings["hourly_payment"] = new Nullable<double>();
                return (Double?)IsolatedStorageSettings.ApplicationSettings["hourly_payment"];
            }
            set
            {
                if (HourlyPayment != value)
                    IsolatedStorageSettings.ApplicationSettings["hourly_payment"] = value;
            }
        }

        public static Double CallPay
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("call_pay"))
                    IsolatedStorageSettings.ApplicationSettings["call_pay"] = 0.0;
                return (Double)IsolatedStorageSettings.ApplicationSettings["call_pay"];
            }
            set
            {
                if (CallPay != value)
                    IsolatedStorageSettings.ApplicationSettings["call_pay"] = value;
            }
        }

        public enum Status
        { 
            Started,
            Stopped,
            Paused,
            Resumed
        }

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

        public static DateTime PauseTime
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("pause_time"))
                    IsolatedStorageSettings.ApplicationSettings["pause_time"] = DateTime.Now;
                return (DateTime)IsolatedStorageSettings.ApplicationSettings["pause_time"];
            }
            set
            {
                if (PauseTime != value)
                    IsolatedStorageSettings.ApplicationSettings["pause_time"] = value;
            }
        }


        public static TimeSpan Threshold
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("threshold"))
                    IsolatedStorageSettings.ApplicationSettings["threshold"] = TimeSpan.FromMinutes(15);
                return (TimeSpan)IsolatedStorageSettings.ApplicationSettings["threshold"];
            }
            set
            {
                if (Threshold != value)
                    IsolatedStorageSettings.ApplicationSettings["threshold"] = value;
            }
        }
    }
}