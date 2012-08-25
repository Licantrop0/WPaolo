using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Scudetti.Model;
using Windows.UI.Xaml.Data;

namespace SocceramaWin8.ViewModel
{
    public class LevelViewModel : ViewModelBase
    {
        int _levelNumber;
        public int LevelNumber
        {
            get { return _levelNumber; }
            set 
            {
                _levelNumber = value;
                RaisePropertyChanged("LevelNumber");
                RaisePropertyChanged("LevelName");
            }
        }

        public string LevelName { get { return "Livello " + LevelNumber; } }

        private IEnumerable<Shield> _shields;
        public IEnumerable<Shield> Shields
        {
            get { return _shields; }
            set
            {
                _shields = value;
                RaisePropertyChanged("Shields");
            }
        }

        public LevelViewModel()
        {
            if (IsInDesignMode)
            {
                LevelNumber = 1;
            }
        }
        public LevelViewModel(IGrouping<int, Shield> level)
        {
            LevelNumber = level.Key;
            Shields = level;
        }
    }
}
