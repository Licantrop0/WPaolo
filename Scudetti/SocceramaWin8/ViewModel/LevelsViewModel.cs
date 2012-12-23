using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using SocceramaWin8.Data;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Resources;
using SocceramaWin8.Sound;

namespace SocceramaWin8.ViewModel
{
    public class LevelsViewModel : ViewModelBase
    {
        ResourceLoader resources = new ResourceLoader();

        private List<LevelViewModel> _levels;
        public List<LevelViewModel> Levels
        {
            get
            {
                if (AppContext.Shields == null)
                {
                    _levels = null;
                    return null;
                }

                if (_levels == null)
                {
                    _levels = AppContext.Shields.GroupBy(s => s.Level)
                        .Select(g => new LevelViewModel(g))
                        .OrderBy(l => l.Number)
                        .ToList();
                }
                return _levels;
            }
        }

        public string StatusText
        {
            get
            {
                return AppContext.Shields == null ? string.Empty :
                    string.Format("{0}: {1}/{2}", resources.GetString("Shields"),
                        AppContext.TotalShieldUnlocked, AppContext.TotalShields);
            }
        }

        public LevelViewModel SelectedLevel
        {
            get { return null; }
            set
            {
                if (value == null) return;
                value.GoToLevelCommand.Execute(null);
                RaisePropertyChanged("SelectedLevel");
            }
        }

        public LevelsViewModel()
        {
            MessengerInstance.Register<PropertyChangedMessage<bool>>(this, m =>
            {
                if (m.PropertyName != "IsValidated") return;
                //Aggiorna lo status text dei completed shields e se il gioco è stato completato
                RaisePropertyChanged("StatusText");
                AppContext.GameCompleted = AppContext.Shields.All(s => s.IsValidated);
            });
        }
    }
}
