using System;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework.Audio;

namespace FillTheSquare
{
    public static class Settings
    {
        public static SoundEffect MoveSound;
        public static SoundEffect ErrorSound;
        public static SoundEffect UndoSound;
        public static SoundEffect VictorySound;

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
            for (int i = 0; i < 30; i++)
            {
                Records.Add(new Record(
                    (i % 2 + 1) * 5,
                    DateTime.Now.AddDays(-rnd.Next(365)).AddHours(-rnd.Next(24)).AddMinutes(-rnd.Next(60)),
                    TimeSpan.FromSeconds(rnd.Next(1000) + 8)));
            }
        }

    }
}