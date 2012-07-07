using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;

namespace Scudetti.ViewModel
{
    public class LevelsViewModel : ViewModelBase
    {
        private IEnumerable<int> _levels;
        public IEnumerable<int> Levels
        {
            get
            {
                if (_levels == null)
                    _levels = AppContext.Shields.Select(s => s.Level).Distinct();
                return _levels;
            }
            private set { _levels = value; }
        }


        public int? SelectedLevel
        {
            get { return null; }
            set
            {
                MessengerInstance.Send<Uri>(new Uri("/View/ShieldsPage.xaml?level="
                    + value, UriKind.Relative), "navigation");
                RaisePropertyChanged("SelectedLevel");
            }
        }

        public LevelsViewModel()
        {
            if (IsInDesignMode)
            {
                Levels = Enumerable.Range(1, 10);
            }
        }
    }
}
