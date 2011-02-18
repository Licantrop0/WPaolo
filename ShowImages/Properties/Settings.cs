using System;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using Microsoft.Phone.Shell;
using System.Linq;
using System.Collections.ObjectModel;

namespace ShowImages
{
    public static class Settings
    {
        public static ObservableCollection<SelectionableImage> CurrentImageList
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("current_image_list") ||
                    (IsolatedStorageSettings.ApplicationSettings["current_image_list"] as ObservableCollection<SelectionableImage>) == null)
                    IsolatedStorageSettings.ApplicationSettings["current_image_list"] = new ObservableCollection<SelectionableImage>();
                return (ObservableCollection<SelectionableImage>)IsolatedStorageSettings.ApplicationSettings["current_image_list"];
            }
            set
            {
                if (CurrentImageList != value)
                    IsolatedStorageSettings.ApplicationSettings["current_image_list"] = value;
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
                set
                {
                    if (History != value)
                        IsolatedStorageSettings.ApplicationSettings["browser_history"] = value;
                }
            }

            public static void Push(Uri uri)
            {
                History.Add(uri);

                //tronco la History al 20° item
                if (History.Count > 20)
                    History = History.Skip(10).ToList();
            }

            public static Uri Pop()
            {
                var last = History.Last();
                History.Remove(last);
                return last;
            }
        }

    }

}