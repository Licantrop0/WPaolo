using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using Scudetti.Data;

namespace Scudetti.ViewModel
{
    public class LevelsViewModel : ViewModelBase
    {
        public List<LevelViewModel> Levels
        {
            get
            {
                return IsInDesignMode ?  DesignTimeData.Levels : AppContext.Levels;
            }
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
    }
}
