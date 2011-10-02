using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using WPCommon.CommonModel;

namespace FillTheSquare.ViewModel
{
    public class AchievementViewModel
    {
        public ObservableCollection<Achievement> Achievements
        {
            get
            {
                if (DesignerProperties.IsInDesignTool)
                    return GetDefaultAchievements();
                else
                {
                    if (!IsolatedStorageSettings.ApplicationSettings.Contains("achievements"))
                        IsolatedStorageSettings.ApplicationSettings.Add("achievements", GetDefaultAchievements());
                    return IsolatedStorageSettings.ApplicationSettings["achievements"] as ObservableCollection<Achievement>;
                }
            }
            set
            {
                if (Achievements == value)
                    return;
                IsolatedStorageSettings.ApplicationSettings["achievements"] = value;
            }
        }

        private ObservableCollection<Achievement> GetDefaultAchievements()
        {
            return new ObservableCollection<Achievement>()
            {
                new Achievement("Supercazzola", "fai la supercazzola", new Uri("http://a4.mzstatic.com/us/r1000/039/Purple/87/38/69/mzl.ljawtloi.175x175-75.jpg")) { IsUnlocked = true },
                new Achievement("5x5", "finisci il 5x5", new Uri("http://a4.mzstatic.com/us/r1000/039/Purple/87/38/69/mzl.ljawtloi.175x175-75.jpg")),
                new Achievement("10x10", "finisci il 10x10", new Uri("http://a4.mzstatic.com/us/r1000/039/Purple/87/38/69/mzl.ljawtloi.175x175-75.jpg")) { IsUnlocked = true },
                new Achievement("tempo limite", "finisci in 3 secondi", new Uri("http://a4.mzstatic.com/us/r1000/039/Purple/87/38/69/mzl.ljawtloi.175x175-75.jpg"))
            };
        }
    }
}
