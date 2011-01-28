using System;
using System.IO.IsolatedStorage;
using System.Globalization;
using Microsoft.Phone.Marketplace;
using System.Collections.Generic;
using System.ComponentModel;

namespace PayMe
{
    public class Settings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #region User Settings

        public static string CurrencySymbol
        {
            get
            {
                return CultureInfo.CurrentUICulture.NumberFormat.CurrencySymbol;
                //if (!IsolatedStorageSettings.ApplicationSettings.Contains("currency_symbol"))
                //    IsolatedStorageSettings.ApplicationSettings["currency_symbol"] = CultureInfo.CurrentUICulture.NumberFormat.CurrencySymbol;
                //return (string)IsolatedStorageSettings.ApplicationSettings["currency_symbol"];
            }
            //set
            //{
            //    if (CurrencySymbol != value)
            //        IsolatedStorageSettings.ApplicationSettings["currency_symbol"] = value;
            //}
        }  

        public Double? HourlyPayment
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
                {
                    IsolatedStorageSettings.ApplicationSettings["hourly_payment"] = value;
                    NotifyPropertyChanged("HourlyPayment");
                }
            }
        }

        public Double CallPay
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
                {
                    IsolatedStorageSettings.ApplicationSettings["call_pay"] = value;
                    NotifyPropertyChanged("CallPay");
                }
            }
        }

        public TimeSpan Threshold
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