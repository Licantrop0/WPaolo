using System;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using Scudetti.Data;
using Scudetti.Localization;
using Scudetti.Model;

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
                CurrentShield = DesignTimeData.Shields.First();
            }
            ShieldName = string.Empty;
        }

        public void Validate()
        {
            if (string.Compare(CurrentShield.Name, ShieldName, StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                CurrentShield.IsValidated = true;
                MessengerInstance.Send<string>("goback", "navigation");
            }
            else
            {
                MessageBox.Show(AppResources.Wrong);
            }
        }
    }
}

