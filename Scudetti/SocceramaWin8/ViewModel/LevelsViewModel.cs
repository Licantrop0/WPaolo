﻿using GalaSoft.MvvmLight;
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

        public List<LevelViewModel> Levels
        {
            get
            {
                return IsInDesignMode ? DesignTimeData.Levels : AppContext.Levels;
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
