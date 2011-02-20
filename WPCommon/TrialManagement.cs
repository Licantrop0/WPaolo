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
using Microsoft.Phone.Marketplace;
using System.IO.IsolatedStorage;

namespace WPCommon
{
    public static class TrialManagement
    {
        private static LicenseInformation li = new LicenseInformation();
        public static bool IsTrialMode { get { return li.IsTrial(); } }
        //public static bool IsTrialMode { get { return true; } }

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
    }
}
