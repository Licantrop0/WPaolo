using System.Collections.Generic;
using GalaSoft.MvvmLight;
using Scudetti.Model;
using System.Linq;
using NascondiChiappe.Helpers;
using System;
using Scudetti.Localization;
using Scudetti.Data;

namespace Scudetti.ViewModel
{
    public class ShieldsViewModel : ViewModelBase
    {
        public int Level { get; set; }
        public string LevelText { get { return string.Format("{0} {1}", AppResources.Level, Level); } }

        private IEnumerable<Shield> _shields;
        public IEnumerable<Shield> Shields
        {
            get
            {
                if (_shields == null)
                    _shields = AppContext.Shields.Where(s => s.Level == Level);
                return _shields;
            }
        }

        public string Progress
        {
            get
            {
                return string.Format("{0} {1}/{2}",
                    AppResources.Shields,
                    Shields.Count(s => s.IsUnlocked),
                    Shields.Count());
            }
        }


        public Shield SelectedShield
        {
            get { return null; }
            set
            {
                if (value.IsUnlocked) return;
                MessengerInstance.Send<Uri>(new Uri("/View/ShieldPage.xaml?id="
                    + value.Id, UriKind.Relative), "navigation");
                RaisePropertyChanged("SelectedShield");
            }
        }

        public ShieldsViewModel()
        {
            if (IsInDesignMode)
            {
                Level = 1;
                _shields = DesignTimeData.GetShields().Where(s => s.Level == Level);
            }

            MessengerInstance.Register<String>(this, (m) =>
            {
                if (m == "UpdateProgress")
                    RaisePropertyChanged("Progress");
            });
        }
    }
}
