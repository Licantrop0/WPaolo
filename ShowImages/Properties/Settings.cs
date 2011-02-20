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

        public static Stack<Uri> BrowserHistory
        {
            get
            {
                if (!PhoneApplicationService.Current.State.ContainsKey("browser_history"))
                    PhoneApplicationService.Current.State["browser_history"] = 
                        new Stack<Uri>(new List<Uri>() { HomePage });

                return (Stack<Uri>)PhoneApplicationService.Current.State["browser_history"];
            }
            set
            {
                if (BrowserHistory != value)
                    PhoneApplicationService.Current.State["browser_history"] = value;
            }
        }
    }

}