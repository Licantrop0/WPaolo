using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using NientePanico.ViewModel;
using NientePanico.Model;

namespace NientePanico
{
    public static class AppContext
    {
        //TODO: Implementare encription
        public static string Password { get; set; }
        public static ObservableCollection<CardData> Cards { get; set; }

        internal static void SaveData()
        {
            IsolatedStorageSettings.ApplicationSettings["password"] = Password;
            IsolatedStorageSettings.ApplicationSettings["cards"] = Cards;
        }

        internal static void RestoreData()
        {
            if (!IsolatedStorageSettings.ApplicationSettings.Contains("password"))
                IsolatedStorageSettings.ApplicationSettings.Add("password", null);
            if (Password == null)
                Password = (string)IsolatedStorageSettings.ApplicationSettings["password"];

            if (!IsolatedStorageSettings.ApplicationSettings.Contains("cards"))
                IsolatedStorageSettings.ApplicationSettings.Add("cards", new ObservableCollection<CardData>());
            if(Cards == null)
                Cards = (ObservableCollection<CardData>)IsolatedStorageSettings.ApplicationSettings["cards"];
        }


    }
}
