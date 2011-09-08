using System;
using System.IO.IsolatedStorage;
using System.Globalization;

namespace Capra
{
    public static class Settings
    {
        public static int TotCapre
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("tot_capre"))
                    IsolatedStorageSettings.ApplicationSettings["tot_capre"] = 0;
                return (int)IsolatedStorageSettings.ApplicationSettings["tot_capre"];
            }
            set
            {
                if (TotCapre != value)
                    IsolatedStorageSettings.ApplicationSettings["tot_capre"] = value;
            }

        }

    }
}