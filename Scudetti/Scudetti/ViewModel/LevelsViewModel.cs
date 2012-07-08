using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using Scudetti.Model;
using Scudetti.Data;

namespace Scudetti.ViewModel
{
    public class LevelsViewModel : ViewModelBase
    {
        private IEnumerable<Level> _levels;
        public IEnumerable<Level> Levels
        {
            get
            {
                if (_levels == null)
                {
                    _levels = AppContext.Shields
                        .GroupBy(s => s.Level)
                        .Select(g => new Level(g));
                }
                return _levels;
            }
            private set { _levels = value; }
        }

        public int TotalShieldUnlocked
        { get { return Levels.Sum(l => l.Completed); } }

        public Level SelectedLevel
        {
            get { return null; }
            set
            {
                MessengerInstance.Send<Uri>(new Uri("/View/ShieldsPage.xaml?level="
                    + value.Key, UriKind.Relative), "navigation");
                RaisePropertyChanged("SelectedLevel");
            }
        }

        public LevelsViewModel()
        {
            if (IsInDesignMode)
            {
                Levels = DesignTimeData.GetShields()
                        .GroupBy(s => s.Level)
                        .Select(g => new Level(g));
            }

            //Level Unlocking
            foreach (var level in Levels)
            {
                if (level.Key == 1)
                {
                    level.IsUnlocked = true;
                }
                else
                {
                    level.PropertyChanged += (sender, e) =>
                    {
                        if (e.PropertyName == "Completed")
                            level.IsUnlocked = TotalShieldUnlocked == (level.Key - 1) * 15;
                    };
                }
            }
        }
    }
}
