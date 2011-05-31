using System;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using WPCommon;
using Microsoft.Phone.Shell;
using System.Windows;
using FillTheSquare.Localization;

namespace FillTheSquare
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

        public static GridPoint[] GetGridState()
        {
            if (!PhoneApplicationService.Current.State.ContainsKey("grid_state"))
                PhoneApplicationService.Current.State.Add("grid_state", new GridPoint[0]);

            return (GridPoint[])PhoneApplicationService.Current.State["grid_state"];
        }

        public static void SetGridState(Stack<GridPoint> state)
        {
            var gp = state.ToArray();
            Array.Reverse(gp);
            PhoneApplicationService.Current.State["grid_state"] = gp;
        }

        public static TimeSpan CurrentElapsedTime
        {
            get
            {
                if (!PhoneApplicationService.Current.State.ContainsKey("current_elapsed_time"))
                    PhoneApplicationService.Current.State.Add("current_elapsed_time", new TimeSpan());

                return (TimeSpan)PhoneApplicationService.Current.State["current_elapsed_time"];
            }
            set
            {
                PhoneApplicationService.Current.State["current_elapsed_time"] = value;
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
                    TimeSpan.FromMilliseconds(rnd.Next(100000) + 8000)) { Name = "Phil" + i });
            }

            Records.AddRange(temp.OrderBy(r => r.Date));
        }

    }
}