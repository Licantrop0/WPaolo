using System;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NascondiChiappe
{
    public static class Settings
    {
        //TODO: Modificare prima del rilascio
        public const int PasswordMinLenght = 1;
        public static bool IsPasswordInserted = false;

        public static string Password
        { //TODO: Implementare encription
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("password"))
                    IsolatedStorageSettings.ApplicationSettings["password"] = null;
                return (string)IsolatedStorageSettings.ApplicationSettings["password"];
            }
            set
            {
                if (Password != value)
                    IsolatedStorageSettings.ApplicationSettings["password"] = value;
            }
        }

        private static ObservableCollection<Album> _albums;

        public static ObservableCollection<Album> Albums
        {
            get
            {
                if (_albums == null)
                    _albums = new ObservableCollection<Album>();
                return Settings._albums;
            }
            set { Settings._albums = value; }
        }

        //public static ObservableCollection<Album> Albums
        //{
        //    get
        //    {
        //        if (!IsolatedStorageSettings.ApplicationSettings.Contains("albums"))
        //            IsolatedStorageSettings.ApplicationSettings["albums"] = new ObservableCollection<Album>();
        //        return (ObservableCollection<Album>)IsolatedStorageSettings.ApplicationSettings["albums"];
        //    }
        //    set
        //    {
        //        if (Albums != value)
        //            IsolatedStorageSettings.ApplicationSettings["albums"] = value;
        //    }
        //}
    }
}
