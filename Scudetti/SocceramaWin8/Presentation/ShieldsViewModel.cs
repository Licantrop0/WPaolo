using Scudetti.Model;
using SocceramaWin8.Data;
using Topics.Radical.ComponentModel.Messaging;
using Topics.Radical.Windows.Presentation;
using Topics.Radical.Windows.Presentation.ComponentModel;
using Windows.ApplicationModel.Resources;

namespace SocceramaWin8.Presentation
{
    public class ShieldsViewModel : AbstractViewModel,
        IExpectNavigatedToCallback, IExpectNavigatingAwayCallback
    {
        ResourceLoader resources = new ResourceLoader();
        readonly INavigationService _ns;

        private LevelViewModel _selectedLevel;
        public LevelViewModel SelectedLevel
        {
            get { return _selectedLevel; }
            set
            {
                if (SelectedLevel == value) return;
                _selectedLevel = value;
                OnPropertyChanged("SelectedLevel");
            }
        }

        public Shield SelectedShield
        {
            get { return null; }
            set
            {
                if (value == null) return;
                if (value.Id.StartsWith("blocked")) return;

                _ns.Navigate<ShieldView>(value);

                OnPropertyChanged("SelectedShield");
            }
        }

        public ShieldsViewModel(INavigationService ns)
        {
            _ns = ns;
            //if (IsInDesignMode)
            //{
            //    SelectedLevel = DesignTimeData.Levels[0];
            //    return;
            //}
        }

        void IExpectNavigatedToCallback.OnNavigatedTo(NavigationEventArgs e)
        {
            SelectedLevel = (LevelViewModel)e.Arguments;
            //if (e.Mode == NavigationMode.Back && e.Storage.Contains("myData"))
            //{
            //    var data = e.Storage.GetData<string>("myData");
            //}
        }

        void IExpectNavigatingAwayCallback.OnNavigatingAway(NavigatingAwayEventArgs e)
        {
            
        }
    }
}
