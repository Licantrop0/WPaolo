using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using SocceramaWin8.Helpers;
using SocceramaWin8.Sound;
using Windows.ApplicationModel.Resources;

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
                    _levels = AppContext.Shields
                        .Select(s=> new ShieldViewModel(s))
                        .GroupBy(s => s.CurrentShield.Level)
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
                    string.Format(resources.GetString("ShieldUnlocked"),
                    AppContext.TotalShieldUnlocked, AppContext.TotalShields);
            }
        }

        public LevelsViewModel()
        {
            MessengerInstance.Register<PropertyChangedMessage<bool>>(this, m =>
            {
                //Aggiorna lo status text dei completed shields e se il gioco è stato completato
                if (m.PropertyName == "IsValidated")
                    ShowNotifications();
            });
        }

        private void ShowNotifications()
        {
            //Se non ho sbloccato alcun scudetto
            if (AppContext.TotalShieldUnlocked == 0) return;

            RaisePropertyChanged("StatusText");

            //Aggiorno la Tile
            NotificationHelper.UpdateTile(StatusText);

            var newLevelUnlocked = AppContext.TotalShieldUnlocked % AppContext.LockTreshold == 0;
            var newBonusLevelUnlocked = AppContext.TotalShieldUnlocked % AppContext.BonusTreshold == 0;

            if (newLevelUnlocked)
            {
                int newLevelNumber = (AppContext.TotalShieldUnlocked / AppContext.LockTreshold) + 1;
                if (newLevelNumber <= 10)
                {
                    var Message = string.Format(resources.GetString("NewLevel"), newLevelNumber);
                    var level = Levels.Single(l => l.Number == newLevelNumber);
                    
                    SoundManager.PlayGoal();
                    NotificationHelper.DisplayToast(Message, level);
                }
            }
            else if (newBonusLevelUnlocked)
            {
                int newBonusLevelNumber = ((AppContext.TotalShieldUnlocked / AppContext.BonusTreshold));
                var Message = string.Format(resources.GetString("NewBonusLevel"), newBonusLevelNumber);
                var bounsLevel = Levels.Single(l => l.Number == newBonusLevelNumber);
                
                SoundManager.PlayGoal();
                NotificationHelper.DisplayToast(Message, bounsLevel);
            }
            else if (AppContext.GameCompleted) //Gioco completato!
            {
                SoundManager.PlayGoal();
                NotificationHelper.DisplayToast(resources.GetString("GameFinished"), null);
                //Title = AppResources.GameFinishedTitle,
                //Message = AppResources.GameFinished,
                //TextWrapping = TextWrapping.Wrap,
                //MillisecondsUntilHidden = 8000,
            }
        }

    }
}
