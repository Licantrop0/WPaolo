using GalaSoft.MvvmLight;
using Scudetti.Model;
using SocceramaWin8.Data;
using Windows.ApplicationModel.Resources;

namespace SocceramaWin8.ViewModel
{
    public class ShieldsViewModel : ViewModelBase
    {
        ResourceLoader resources = new ResourceLoader();

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

        public Shield SelectedShield
        {
            get { return null; }
            set
            {
                if (value == null) return;
                MessengerInstance.Send<Shield>(value);
            }
        }

        public ShieldsViewModel()
        {
            if (IsInDesignMode)
            {
                SelectedLevel = DesignTimeData.Levels[0];
                return;
            }

            MessengerInstance.Register<LevelViewModel>(this, m =>
            {
                SelectedLevel = m;
            });
        }
    }
}
