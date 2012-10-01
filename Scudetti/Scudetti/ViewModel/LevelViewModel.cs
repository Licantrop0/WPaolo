using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Scudetti.Data;
using Scudetti.Localization;
using Scudetti.Model;
using Scudetti.Sound;

namespace Scudetti.ViewModel
{
    public class LevelViewModel : ViewModelBase
    {
        public int Number { get; private set; }
        public bool IsBonus { get; set; }
        public string LevelName
        {
            get
            {
                return IsBonus ?
                    string.Format("{0} {1}", AppResources.BonusLevel, Number / 100) :
                    string.Format("{0} {1}", AppResources.Level, Number);
            }
        }
        public IEnumerable<Shield> Shields { get; private set; }
        public int TotalShields { get { return Shields.Count(); } }
        public int CompletedShields { get { return Shields.Count(s => s.IsValidated); } }

        public bool IsUnlocked
        {
            get
            {
                if (IsBonus)
                    return AppContext.TotalShieldUnlocked >= Number / 100 * AppContext.BonusTreshold;
                else
                    return Number == 1 || AppContext.TotalShieldUnlocked >= (Number - 1) * AppContext.LockTreshold;
            }
        }

        public Uri LevelImage
        {
            get
            {
                return IsUnlocked ?
                    new Uri("../Images/Levels/livello" + Number + ".png", UriKind.Relative) :
                    new Uri("../Images/Levels/livello" + Number + "lock.png", UriKind.Relative);
            }
        }

        public string StatusText
        {
            get
            {
                return IsUnlocked ?
                    string.Format("{0}/{1}", CompletedShields, TotalShields) :
                    IsBonus ?
                        (AppContext.BonusTreshold * (Number / 100) - AppContext.TotalShieldUnlocked).ToString() :
                        (AppContext.LockTreshold * (Number - 1) - AppContext.TotalShieldUnlocked).ToString();
            }
        }


        public LevelViewModel()
            : this(1, DesignTimeData.Shields.Where(s => s.Level == 1))
        { }

        public LevelViewModel(IGrouping<int, Shield> group)
            : this(group.Key, group)
        { }

        public LevelViewModel(int number, IEnumerable<Shield> shields)
        {
            Number = number;
            IsBonus = number >= 100;
            Shields = shields;
            if (!IsInDesignMode) return;
            MessengerInstance.Register<PropertyChangedMessage<bool>>(this, (m) =>
            {
                if (m.PropertyName != "IsValidated") return;

                RaisePropertyChanged("CompletedShields");
                RaisePropertyChanged("IsUnlocked");
                RaisePropertyChanged("StatusText");
                RaisePropertyChanged("LevelImage");
            });
        }
    }
}
