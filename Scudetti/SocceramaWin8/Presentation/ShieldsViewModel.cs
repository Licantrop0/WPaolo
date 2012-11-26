using Scudetti.Model;
using SocceramaWin8.Data;
using Topics.Radical.Windows.Presentation;
using Topics.Radical.Windows.Presentation.ComponentModel;
using Windows.ApplicationModel.Resources;

namespace SocceramaWin8.Presentation
{
    public class ShieldsViewModel : AbstractViewModel, IExpectNavigatedToCallback
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
        }

        public void OnNavigatedTo(NavigationEventArgs e)
        {
            SelectedLevel = (LevelViewModel)e.Arguments;            
        }
    }
}
