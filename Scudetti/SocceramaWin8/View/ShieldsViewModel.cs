using Scudetti.Model;
using SocceramaWin8.Data;
using Windows.ApplicationModel.Resources;

namespace SocceramaWin8.View
{
    public class ShieldsViewModel
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
                //RaisePropertyChanged("SelectedLevel");
            }
        }

        public Shield SelectedShield
        {
            get { return null; }
            set
            {
                if (value == null) return;
                //if (!value.Id.StartsWith("blocked"))
                //    MessengerInstance.Send<Shield>(value);
                
                //RaisePropertyChanged("SelectedShield");
            }
        }

        public ShieldsViewModel()
        {
            //if (IsInDesignMode)
            //{
            //    SelectedLevel = DesignTimeData.Levels[0];
            //    return;
            //}

            //MessengerInstance.Register<LevelViewModel>(this, m =>
            //{
            //    SelectedLevel = m;
            //});
        }
    }
}
