using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Scudetti.Model;
using Windows.UI.Xaml.Data;
using GalaSoft.MvvmLight.Messaging;
using Windows.ApplicationModel.Resources;

namespace SocceramaWin8.ViewModel
{
    public class LevelsViewModel : ViewModelBase
    {
        ResourceLoader resources = new ResourceLoader();

        public List<LevelViewModel> Levels
        {
            get
            {
                return AppContext.Levels;
            }
        }

        public LevelViewModel SelectedLevel
        {
            get { return null; }
            set
            {
                if (value == null) return;
                if (value.IsUnlocked)
                {
                    //SoundManager.PlayFischietto();
                    MessengerInstance.Send<LevelViewModel>(value);
                }
                RaisePropertyChanged("SelectedLevel");
            }
        }

        public string StatusText
        {
            get
            {
                return AppContext.Shields == null ? string.Empty :
                    string.Format("{0}: {1}/{2}", resources.GetString("Shields"),
                        AppContext.TotalShieldUnlocked, AppContext.Shields.Count());
            }
        }

        public LevelsViewModel()
        {
            AppContext.LoadCompleted += (sender, e) =>
            {
                RaisePropertyChanged("Levels");
                RaisePropertyChanged("StatusText");
            };

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
