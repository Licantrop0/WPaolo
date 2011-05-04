using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.Linq;
using Microsoft.Phone.Shell;
using WPCommon;

namespace SortTheSquare
{
    public static class Settings
    {
        public static int CurrentGridSize
        {
            get
            {

                if (!PhoneApplicationService.Current.State.ContainsKey("current_grid_size"))
                    PhoneApplicationService.Current.State.Add("current_grid_size", 5);

                return (int)PhoneApplicationService.Current.State["current_grid_size"];
            }
            set
            {
                PhoneApplicationService.Current.State["current_grid_size"] = value;
            }
        }

        public static ObservableCollection<Record> Records
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("records"))
                    IsolatedStorageSettings.ApplicationSettings["records"] = new ObservableCollection<Record>();
                return (ObservableCollection<Record>)IsolatedStorageSettings.ApplicationSettings["records"];
            }
            set
            {
                if (Records != value)
                    IsolatedStorageSettings.ApplicationSettings["records"] = value;
            }
        }

        public static void AddFakeRecords()
        {
            if (Records.Count >= 30) return;

            var rnd = new Random();
            var temp = new List<Record>();
            for (int i = 0; i < 30; i++)
            {
                temp.Add(new Record((i % 2 + 1) * 5,
                    DateTime.Now.AddDays(-rnd.Next(365)).AddHours(-rnd.Next(24)).AddMinutes(-rnd.Next(60)),
                    TimeSpan.FromSeconds(rnd.Next(1000) + 8)) { Name = "Phil" + i });
            }

            Records.AddRange(temp.OrderBy(r => r.Date));
        }

    }
}