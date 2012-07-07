using System.Collections.Generic;
using GalaSoft.MvvmLight;
using Scudetti.Model;
using System.Linq;
using NascondiChiappe.Helpers;
using System;

namespace Scudetti.ViewModel
{
    public class ShieldsViewModel : ViewModelBase
    {
        public int Level { get; set; }
        public string LevelText { get { return "Livello " + Level; } }

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
                return "Scudetti " + Shields.Count(s => s.IsValidated) + " / " + Shields.Count();
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

        public ShieldsViewModel()
        {
            if (IsInDesignMode)
            {
            }

            MessengerInstance.Register<String>(this, (m) =>
            {
                if (m == "UpdateProgress")
                    RaisePropertyChanged("Progress");
            });
        }
    }
}
