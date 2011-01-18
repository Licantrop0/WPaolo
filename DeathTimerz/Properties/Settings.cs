using System;
using System.IO.IsolatedStorage;

namespace DeathTimer
{
    public static class Settings
    {
        public const double AverageYear = 365.25;
        public const double AverageMonth = 30.4368499;

        public static DateTime? BirthDay
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("birthday"))
                    IsolatedStorageSettings.ApplicationSettings["birthday"] = null;
                return (DateTime?)IsolatedStorageSettings.ApplicationSettings["birthday"];
            }
            set
            {
                if (BirthDay != value)
                    IsolatedStorageSettings.ApplicationSettings["birthday"] = value;
            }
        }

        public static TimeSpan? EstimatedDeathAge
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("estimated_death_age"))
                    IsolatedStorageSettings.ApplicationSettings["estimated_death_age"] = null;
                return (TimeSpan?)IsolatedStorageSettings.ApplicationSettings["estimated_death_age"];
            }
            set
            {
                if (EstimatedDeathAge != value)
                    IsolatedStorageSettings.ApplicationSettings["estimated_death_age"] = value;
            }
        }
    }
}
