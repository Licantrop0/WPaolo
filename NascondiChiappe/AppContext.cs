using System;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NascondiChiappe
{
    public static class AppContext
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

        public static List<Album> Albums { get; set; }
        public static Album CurrentAlbum { get; set; }
    }
}
