using GalaSoft.MvvmLight;
using Scudetti.Data;
using Scudetti.Model;
using Scudetti.Sound;

namespace Scudetti.ViewModel
{
    public class ShieldsViewModel : ViewModelBase
    {
        private LevelViewModel _selectedLevel;
        public LevelViewModel SelectedLevel
        {
            get { return _selectedLevel; }
            set
            {
                if (SelectedLevel == value) return;
                _selectedLevel = value;
                RaisePropertyChanged("SelectedLevel");
            }
        }

        public ShieldsViewModel()
        {
            if (IsInDesignMode)
            {
                SelectedLevel = DesignTimeData.Levels[0];
                return;
            }

            MessengerInstance.Register<LevelViewModel>(this, m => SelectedLevel = m);
        }
    }
}
