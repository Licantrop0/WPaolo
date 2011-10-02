using System;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Marketplace;

namespace WPCommon.Helpers
{
    public static class TrialManagement
    {
        private static LicenseInformation li = new LicenseInformation();
        public static bool IsTrialMode
        {
            get
            {
#if DEBUG
                return true;
#else
                return li.IsTrial();
#endif
            }
        }

        public static int Counter
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("counter"))
                    IsolatedStorageSettings.ApplicationSettings.Add("counter", 0);

                return (int)IsolatedStorageSettings.ApplicationSettings["counter"];
            }
        }

        public static void IncrementCounter()
        {
            IsolatedStorageSettings.ApplicationSettings["counter"] = Counter + 1;
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

        private static DateTime LastOpen
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("last_open"))
                    IsolatedStorageSettings.ApplicationSettings.Add("last_open", DateTime.Today.AddDays(-1));

                return (DateTime)IsolatedStorageSettings.ApplicationSettings["last_open"];
            }
            set
            {
                if (LastOpen != value)
                    IsolatedStorageSettings.ApplicationSettings["last_open"] = value;
            }
        }


    }
}
