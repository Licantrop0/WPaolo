using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.Linq;
using Microsoft.Phone.Shell;

namespace ShowImages
{
    public static class Settings
    {
        public static readonly Uri HomePage = new Uri("http://www.google.com/m/?site=images");

        public static ObservableCollection<SelectionableImage> CurrentImageList
        {
            get
            {
                if (!PhoneApplicationService.Current.State.ContainsKey("current_image_list"))
                    PhoneApplicationService.Current.State["current_image_list"] = new ObservableCollection<SelectionableImage>();
                return (ObservableCollection<SelectionableImage>)PhoneApplicationService.Current.State["current_image_list"];
            }
            set
            {
                if (CurrentImageList != value)
                    PhoneApplicationService.Current.State["current_image_list"] = value;
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
                            new List<Uri> { HomePage };

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