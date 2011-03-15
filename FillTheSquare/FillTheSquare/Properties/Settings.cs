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
    }
}