using System;
using System.IO.IsolatedStorage;

namespace NascondiChiappe
{
    public static class Settings
    {
        public const int PasswordMinLenght = 6;
        //Implementare encription
        public static string Password
        {
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
    }
}
