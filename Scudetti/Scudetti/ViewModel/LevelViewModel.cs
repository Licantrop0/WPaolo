using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using Scudetti.Data;
using Scudetti.Helper;
using Scudetti.Localization;
using Scudetti.Model;

namespace Scudetti.ViewModel
{
    public class LevelViewModel : ViewModelBase
    {
        public int Number { get; private set; }
        public string LevelName { get { return string.Format("{0} {1}", AppResources.Level, Number); } }
        public IEnumerable<Shield> Shields { get; private set; }
        public int TotalShields { get { return Shields.Count(); } }
        public int CompletedShields { get { return Shields.Count(s => s.IsValidated); } }

        private const int LockTreshold = 5;

        public bool IsUnlocked
        {
            get
            {
                return  Number == 1 || AppContext.TotalShieldUnlocked >= (Number - 1) * LockTreshold;
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
                MessengerInstance.Send<Uri>(new Uri("/View/ShieldPage.xaml?id="
                    + value.Id, UriKind.Relative), "navigation");
                RaisePropertyChanged("SelectedShield");
            }
        }

        public LevelViewModel() : this(1, DesignTimeData.Shields.Where(s => s.Level == 1))
        { }

        public LevelViewModel(int number, IEnumerable<Shield> shields)
        {
            if (IsInDesignMode)
            {
                Number = number;
                Shields = shields;
            }
        }

        public LevelViewModel(IGrouping<int, Shield> group)
        {
            Number = group.Key;
            var list = group.ToList();
            list.Shuffle();
            Shields = list;

            foreach (var shield in AppContext.Shields)
            {
                shield.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == "IsValidated")
                    {
                        RaisePropertyChanged("CompletedShields");
                        RaisePropertyChanged("IsUnlocked");
                        RaisePropertyChanged("StatusText");
                    }
                };
            }
        }
    }
}
