using System;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;

namespace FillTheSquare
{
    public static class Settings
    {
        public static ObservableCollection<Record> RecordsList
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("records_list"))
                    IsolatedStorageSettings.ApplicationSettings["records_list"] = new ObservableCollection<Record>();
                return (ObservableCollection<Record>)IsolatedStorageSettings.ApplicationSettings["records_list"];
            }
            set
            {
                if (RecordsList != value)
                    IsolatedStorageSettings.ApplicationSettings["records_list"] = value;
            }
        }
    }
}