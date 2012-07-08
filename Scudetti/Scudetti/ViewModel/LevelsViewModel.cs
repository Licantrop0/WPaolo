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
        private IEnumerable<LevelViewModel> _levels;
        public IEnumerable<LevelViewModel> Levels
        {
            get
            {
                if (_levels == null)
                {
                    _levels = AppContext.Shields
                        .GroupBy(s => s.Level)
                        .Select(g => new LevelViewModel(g));
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
                MessengerInstance.Send<Uri>(new Uri("/View/ShieldsPage.xaml?level="
                    + value.Number, UriKind.Relative), "navigation");
                RaisePropertyChanged("SelectedLevel");
            }
        }

        public LevelsViewModel()
        {
            if (IsInDesignMode)
            {
                Levels = DesignTimeData.GetShields()
                        .GroupBy(s => s.Level)
                        .Select(g => new LevelViewModel(g));
            }
        }
    }
}
