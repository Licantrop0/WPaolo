using System;
using System.IO.IsolatedStorage;
using System.Globalization;
using Microsoft.Phone.Marketplace;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

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

        #region User Settings


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

        public static ObservableCollection<Attendance> Attendances
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("attendances"))
                    IsolatedStorageSettings.ApplicationSettings["attendances"] = new ObservableCollection<Attendance>();
                return (ObservableCollection<Attendance>)IsolatedStorageSettings.ApplicationSettings["attendances"];
            }
            set
            {
                if (Attendances != value)
                    IsolatedStorageSettings.ApplicationSettings["attendances"] = value;
            }
        }

    }
}