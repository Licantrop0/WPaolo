using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Resources;
using System.Xml.Linq;

namespace DeathTimerz
{
    public static class AppContext
    {
        public const double AverageYear = 365.25;
        public const double AverageMonth = 30.4368499;

        public static TimeSpan AverageAge
        {
            get
            {
                return (bool)IsolatedStorageSettings.ApplicationSettings["is_male"] ?
                    ExtensionMethods.TimeSpanFromYears(77.5) :
                    ExtensionMethods.TimeSpanFromYears(83.7);
            }
        }

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
                if (BirthDay == value) return;
                IsolatedStorageSettings.ApplicationSettings["birthday"] = value;
            }
        }

        public static TimeSpan? DeathDeviation
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("estimated_death_age"))
                    IsolatedStorageSettings.ApplicationSettings["estimated_death_age"] = null;
                return (TimeSpan?)IsolatedStorageSettings.ApplicationSettings["estimated_death_age"];
            }
            set
            {
                if (DeathDeviation == value) return;
                IsolatedStorageSettings.ApplicationSettings["estimated_death_age"] = value;
            }
        }

        public static bool PinSuggestionVisible
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("pin_suggestion"))
                    IsolatedStorageSettings.ApplicationSettings["pin_suggestion"] = true;
                return (bool)IsolatedStorageSettings.ApplicationSettings["pin_suggestion"];
            }
            set
            {
                if (PinSuggestionVisible == value) return;
                IsolatedStorageSettings.ApplicationSettings["pin_suggestion"] = value;
            }
        }

        private static IsolatedStorageFile isf
        {
            get { return IsolatedStorageFile.GetUserStoreForApplication(); }
        }

        private static XDocument _test1;
        public static XDocument Test1
        {
            get { return _test1 ?? (_test1 = GetTest("Test1.xml")); }
        }

        private static XDocument _test2;
        public static XDocument Test2
        {
            get { return _test2 ?? (_test2 = GetTest("Test2.xml")); }
        }


        private static XDocument GetTest(string name)
        {
            if (!isf.FileExists(name))
                CopyFileToStorage(name);

            using (var fs = new IsolatedStorageFileStream(name, FileMode.Open, FileAccess.Read, isf))
            {
                return XDocument.Load(fs);
            }
        }

        public static void SaveTest(XDocument test, string name)
        {
            using (var fs = new IsolatedStorageFileStream(name, FileMode.Create, FileAccess.Write, isf))
                test.Save(fs);
        }

        private static void CopyFileToStorage(string fileName)
        {
            using (var fs = new IsolatedStorageFileStream(fileName, FileMode.CreateNew, FileAccess.Write, isf))
            {
                StreamResourceInfo sri = Application.GetResourceStream(
                    new Uri("DeathTimerz;component/" + fileName, UriKind.Relative));
                byte[] bytesInStream = new byte[sri.Stream.Length];
                sri.Stream.Read(bytesInStream, 0, (int)bytesInStream.Length);
                fs.Write(bytesInStream, 0, bytesInStream.Length);
            }
        }

    }
}
