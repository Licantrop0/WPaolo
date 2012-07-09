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
        public string ShieldName { get; set; }
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

        public Visibility InputVisibile { get { return CurrentShield.IsValidated ? Visibility.Collapsed : Visibility.Visible; } }

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

