using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using Coding4Fun.Phone.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Shell;
using Scudetti.Data;
using Scudetti.Localization;
using Scudetti.Model;
using Scudetti.Sound;

namespace Scudetti.ViewModel
{
    public class LevelViewModel : ViewModelBase
    {
        public int Number { get; private set; }
        public string LevelName { get { return string.Format("{0} {1}", AppResources.Level, Number); } }
        public IEnumerable<Shield> Shields { get; private set; }
        public int TotalShields { get { return Shields.Count(); } }
        public int CompletedShields { get { return Shields.Count(s => s.IsValidated); } }

        public bool IsUnlocked
        {
            get
            {
                return  Number == 1 || AppContext.TotalShieldUnlocked >= (Number - 1) * AppContext.LockTreshold;
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
                    (AppContext.LockTreshold * (Number - 1) - AppContext.TotalShieldUnlocked).ToString();
            }
        }

        public Shield SelectedShield
        {
            get { return null; }
            set
            {
                SoundManager.PlayKick();
                MessengerInstance.Send(new Uri("/View/ShieldPage.xaml?id="
                    + value.Id, UriKind.Relative), "navigation");
                RaisePropertyChanged("SelectedShield");
            }
        }

        public LevelViewModel() : this(1, DesignTimeData.Shields.Where(s => s.Level == 1))
        { }

        public LevelViewModel(int number, IEnumerable<Shield> shields)
        {
            if (!IsInDesignMode) return;
            Number = number;
            Shields = shields;
        }

        public LevelViewModel(IGrouping<int, Shield> group)
        {
            Number = group.Key;
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
