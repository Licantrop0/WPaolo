using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Scudetti.Model;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;
using SocceramaWin8.Helpers;

namespace SocceramaWin8.ViewModel
{
    public class LevelViewModel : ViewModelBase
    {
        ResourceLoader resources = new ResourceLoader();

        public int Number { get; private set; }
        public bool IsBonus { get; set; }
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
                    new Uri("ms-appx:/Assets/Levels/livello" + Number + ".png") :
                    new Uri("ms-appx:/Assets/Levels/livello" + Number + "lock.png");
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

        public LevelViewModel(IGrouping<int, Shield> group)
        {
            Number = group.Key;
            IsBonus = group.Key >= 100;
            Shields = group;
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
