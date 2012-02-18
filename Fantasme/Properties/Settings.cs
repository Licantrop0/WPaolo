using System.IO.IsolatedStorage;

namespace NascondiChiappe
{
    public static class Settings
    {
        //TODO: Modificare prima del rilascio
        public const int PasswordMinLenght = 1;

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

    }
}
