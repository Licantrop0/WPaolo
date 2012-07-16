using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using Scudetti.Data;
using Scudetti.Sound;
using System.Linq;
using Scudetti.Localization;
using GalaSoft.MvvmLight.Messaging;

namespace Scudetti.ViewModel
{
    public class LevelsViewModel : ViewModelBase
    {
        public List<LevelViewModel> Levels
        {
            get
            {
                return IsInDesignMode ?  DesignTimeData.Levels : AppContext.Levels;
            }
        }

        public string StatusText
        {
            get
            {
                return AppContext.Shields == null ?
                    AppResources.StartGame :
                    string.Format("{0}: {1}/{2}", AppResources.Shields,
                        AppContext.TotalShieldUnlocked, AppContext.Shields.Count());
            }
        }

        public LevelViewModel SelectedLevel
        {
            get { return null; }
            set
            {
                if (!value.IsUnlocked) return;
                SoundManager.PlayFischietto();
                MessengerInstance.Send<Uri>(new Uri("/View/ShieldsPage.xaml?level="
                    + Levels.IndexOf(value), UriKind.Relative), "navigation");
                RaisePropertyChanged("SelectedLevel");
            }
        }

        public LevelsViewModel()
        {
            AppContext.LoadCompleted += (sender, e) => RaisePropertyChanged("Levels");
            MessengerInstance.Register<PropertyChangedMessage<bool>>(this, (m) =>
            {
                if (m.PropertyName == "IsValidated")
                    RaisePropertyChanged("StatusText");
            });
        }
    }
}
