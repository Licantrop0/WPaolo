using System;
using System.IO.IsolatedStorage;
using System.Globalization;
using Microsoft.Phone.Marketplace;
using System.Collections.Generic;

namespace PayMe
{
    public static class Settings
    {

        #region User Settings

        public static string CurrencySymbol
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("currency_symbol"))
                    IsolatedStorageSettings.ApplicationSettings["currency_symbol"] = CultureInfo.CurrentUICulture.NumberFormat.CurrencySymbol;
                return (string)IsolatedStorageSettings.ApplicationSettings["currency_symbol"];
            }
            set
            {
                if (CurrencySymbol != value)
                    IsolatedStorageSettings.ApplicationSettings["currency_symbol"] = value;
            }
        }  

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

        #endregion

        #region Status Management

        public enum Status
        {
            Stopped = 0,
            Started = 1,
            Paused = 2,
            Resumed = 3
        }

        public static Status CurrentStatus
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("current_status"))
                    IsolatedStorageSettings.ApplicationSettings["current_status"] = 0;
                return (Status)IsolatedStorageSettings.ApplicationSettings["current_status"];
            }
            set
            {
                if (CurrentStatus != value)
                    IsolatedStorageSettings.ApplicationSettings["current_status"] = (int)value;
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

        #endregion

        public static List<Attendance> Attendances
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("attendances"))
                    IsolatedStorageSettings.ApplicationSettings["attendances"] = new List<Attendance>();
                return (List<Attendance>)IsolatedStorageSettings.ApplicationSettings["attendances"];
            }
            set
            {
                if (Attendances != value)
                    IsolatedStorageSettings.ApplicationSettings["attendances"] = value;
            }
        }
        
        #region Trial Mode Detection

        private static LicenseInformation li = new LicenseInformation();
        public static bool IsTrialMode { get { return li.IsTrial(); } }

        private static DateTime LastOpen
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("last_open"))
                    IsolatedStorageSettings.ApplicationSettings["last_open"] = DateTime.Today.AddDays(-1);
                return (DateTime)IsolatedStorageSettings.ApplicationSettings["last_open"];
            }
            set
            {
                if (LastOpen != value)
                    IsolatedStorageSettings.ApplicationSettings["last_open"] = value;
            }
        }

        public static bool AlreadyOpenedToday
        {
            get
            {
                if (LastOpen == DateTime.Today)
                    return true;
                else
                {
                    LastOpen = DateTime.Today;
                    return false;
                }
            }
        }

        #endregion

    }
}