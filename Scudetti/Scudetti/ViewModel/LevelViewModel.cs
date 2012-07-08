using System.Linq;
using GalaSoft.MvvmLight;
using Scudetti.Model;
using System.Collections.Generic;
using Scudetti.Localization;

namespace Scudetti.ViewModel
{
    public class LevelViewModel : ViewModelBase
    {
        public int Number { get; private set; }
        public IEnumerable<Shield> Shields { get; private set; }

        public bool IsUnlocked
        {
            get
            {
                if (Number == 1)
                {
                    return true;
                }
                else
                {
                    return AppContext.TotalShieldUnlocked == (Number - 1) * 15;
                }
            }
        }

        public int TotalShields { get { return Shields.Count(); } }
        public int CompletedShields { get { return Shields.Count(s => s.IsUnlocked); } }
        public string LevelName { get { return string.Format("{0} {1}", AppResources.Level, Number); } }

        public string StatusText
        {
            get
            {
                return IsUnlocked ?
                    string.Format("{0}/{1}", CompletedShields, TotalShields) :
                    string.Format(AppResources.Locked, 15 * (Number - 1) - AppContext.TotalShieldUnlocked);
            }
        }

        public LevelViewModel(IGrouping<int, Shield> group)
        {
            Number = group.Key;
            Shields = group;

            foreach (var shield in Shields)
            {
                shield.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == "IsUnlocked")
                    {
                        RaisePropertyChanged("CompletedShields");
                        RaisePropertyChanged("IsUnlocked");
                    }
                };
            }
        }
    }
}
