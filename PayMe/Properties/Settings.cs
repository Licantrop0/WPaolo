using System;
using System.IO.IsolatedStorage;
using System.Globalization;
using Microsoft.Phone.Marketplace;

namespace PayMe
{
    public static class Settings
    {
        public static string CurrencySymbol { get { return CultureInfo.CurrentUICulture.NumberFormat.CurrencySymbol; } }

        #region User Settings

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

        #endregion

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