using System;
using System.IO.IsolatedStorage;
using System.Xml.Linq;
using System.IO;

namespace DeathTimerz
{
    public static class Settings
    {
        public const double AverageYear = 365.25;
        public const double AverageMonth = 30.4368499;

        public static double AverageAge
        {
            get
            {
                return (bool)IsolatedStorageSettings.ApplicationSettings["is_male"] ? 77.5 : 83.7;
            }
        }

        public static DateTime BirthDay
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("birthday"))
                    IsolatedStorageSettings.ApplicationSettings["birthday"] = DateTime.Now.AddYears(-30);
                return (DateTime)IsolatedStorageSettings.ApplicationSettings["birthday"];
            }
            set
            {
                if (BirthDay == value) return;
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
                if (EstimatedDeathAge == value) return;
                IsolatedStorageSettings.ApplicationSettings["estimated_death_age"] = value;
            }
        }

        private static IsolatedStorageFile isf
        {
            get { return IsolatedStorageFile.GetUserStoreForApplication(); }
        }

        private static XDocument _test1;
        public static XDocument Test1
        {
            get
            {
                if (_test1 == null)
                    using (var fs = new IsolatedStorageFileStream("Test1.xml", FileMode.Open, FileAccess.Read, isf))
                        _test1 = XDocument.Load(fs);

                return _test1;
            }
        }

        private static XDocument _test2;
        public static XDocument Test2
        {
            get
            {
                if (_test2 == null)
                    using (var fs = new IsolatedStorageFileStream("Test2.xml", FileMode.Open, FileAccess.Read, isf))
                        _test2 = XDocument.Load(fs);

                return _test2;
            }
        }

    }
}
