using System.Linq;
using GalaSoft.MvvmLight;
using Scudetti.Model;
using System.Collections.Generic;
using Scudetti.Localization;
using Scudetti.Data;
using System;

namespace Scudetti.ViewModel
{
    public class LevelViewModel : ViewModelBase
    {
        public int Number { get; private set; }
        public string LevelName { get { return string.Format("{0} {1}", AppResources.Level, Number); } }
        public IEnumerable<Shield> Shields { get; private set; }
        public int TotalShields { get { return Shields.Count(); } }
        public int CompletedShields { get { return Shields.Count(s => s.IsValidated); } }

        private int LockTreshold = 3;

        public bool IsUnlocked
        {
            get
            {
                return  Number == 1 ? true :
                    AppContext.TotalShieldUnlocked >= (Number - 1) * LockTreshold;
            }
        }

        public string StatusText
        {
            get
            {
                return IsUnlocked ?
                    string.Format("{0} {1}/{2}", AppResources.Shields, CompletedShields, TotalShields) :
                    string.Format(AppResources.Locked, LockTreshold * (Number - 1) - AppContext.TotalShieldUnlocked);
            }
        }

        public Shield SelectedShield
        {
            get { return null; }
            set
            {
                if (value.IsValidated) return;
                MessengerInstance.Send<Uri>(new Uri("/View/ShieldPage.xaml?id="
                    + value.Id, UriKind.Relative), "navigation");
                RaisePropertyChanged("SelectedShield");
            }
        }

        public LevelViewModel()
        {
            if (IsInDesignMode)
            {
                Number = 1;
                Shields = DesignTimeData.Shields.Where(s => s.Level == Number);
            }
        }

        public LevelViewModel(IGrouping<int, Shield> group)
        {
            Number = group.Key;
            Shields = group;

            foreach (var shield in AppContext.Shields)
            {
                shield.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == "IsValidated")
                    {
                        UpdateLockStatus();
                    }
                };
            }
        }

        public void UpdateLockStatus()
        {
            RaisePropertyChanged("CompletedShields");
            RaisePropertyChanged("IsUnlocked");
            RaisePropertyChanged("StatusText");
        }
    }
}
