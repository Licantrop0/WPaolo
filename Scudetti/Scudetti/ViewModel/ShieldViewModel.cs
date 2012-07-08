using Scudetti.Model;
using System.Linq;
using GalaSoft.MvvmLight;
using NascondiChiappe.Helpers;
using System.Windows;
using System;
using Scudetti.Localization;

namespace Scudetti.ViewModel
{
    public class ShieldViewModel : ViewModelBase
    {
        //public INavigationService NavigationService { get; set; }

        private Shield _currentShield;
        public Shield CurrentShield
        {
            get { return _currentShield; }
            set
            {
                if (CurrentShield == value)
                    return;
                _currentShield = value;
                RaisePropertyChanged("CurrentShield");
            }
        }

        public string ShieldName { get; set; }
        public ShieldViewModel()
        {
            if (IsInDesignMode)
            {
                CurrentShield = new Shield() { Id = 1, Level = 1, Name = "Barcellona", Image = "barcellona.png", IsUnlocked = false };
            }
            ShieldName = string.Empty;
        }

        public void Validate()
        {
            if (string.Compare(CurrentShield.Name, ShieldName, StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                CurrentShield.IsUnlocked = true;
                MessengerInstance.Send<string>("goback", "navigation");
                MessengerInstance.Send<string>("UpdateProgress");
            }
            else
            {
                MessageBox.Show(AppResources.Wrong);
            }
        }
    }
}

