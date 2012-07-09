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
        private List<LevelViewModel> _levels;
        public List<LevelViewModel> Levels
        {
            get
            {
                if (_levels == null)
                {
                    _levels = AppContext.Levels;
                }
                return _levels;
            }
            private set { _levels = value; }
        }

        public LevelViewModel SelectedLevel
        {
            get { return null; }
            set
            {
                if (!value.IsUnlocked) return;
                MessengerInstance.Send<Uri>(new Uri("/View/ShieldsPage.xaml?level="
                    + Levels.IndexOf(value), UriKind.Relative), "navigation");
                RaisePropertyChanged("SelectedLevel");
            }
        }

        public LevelsViewModel()
        {
            if (IsInDesignMode)
            {
                Levels = DesignTimeData.Shields
                        .GroupBy(s => s.Level)
                        .Select(g => new LevelViewModel(g)).ToList();
                return;
            }

        }
    }
}
