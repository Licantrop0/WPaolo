using System;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using Microsoft.Phone.Shell;
using System.Linq;

namespace ShowImages
{
    public static class Settings
    {
        public static bool BrowserVisibility
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("is_browser_visible"))
                    IsolatedStorageSettings.ApplicationSettings["is_browser_visible"] = true;
                return (bool)IsolatedStorageSettings.ApplicationSettings["is_browser_visible"];
            }
            set
            {
                if (BrowserVisibility != value)
                    IsolatedStorageSettings.ApplicationSettings["is_browser_visible"] = value;
            }
        }

        public static class BrowserHistory
        {
            public static bool IsEmpty { get { return History.Count <= 1; } }
            
            private static List<Uri> History
            {
                get
                {
                    if (!IsolatedStorageSettings.ApplicationSettings.Contains("browser_history"))
                        IsolatedStorageSettings.ApplicationSettings["browser_history"] =
                            new List<Uri> { new Uri("http://www.google.com/m/?site=images") };

                    return (List<Uri>)IsolatedStorageSettings.ApplicationSettings["browser_history"];
                }
            }


            public static void Push(Uri uri)
            {
                History.Add(uri);
            }

            public static Uri Pop()
            {
                var last = History.Last();
                History.Remove(last);
                return last;
            }

            public static Uri Peek()
            {
                return History.Last();
            }
        }

    }


}