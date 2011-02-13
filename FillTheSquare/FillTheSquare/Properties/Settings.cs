using System;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework.Audio;

namespace FillTheSquare
{
    public static class Settings
    {
        public static SoundEffect MoveSound;
        public static SoundEffect ErrorSound;
        public static SoundEffect UndoSound;

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