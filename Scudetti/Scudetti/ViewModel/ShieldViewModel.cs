using System;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using Scudetti.Data;
using Scudetti.Localization;
using Scudetti.Model;
using System.Globalization;

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
            var originalName = ShieldResources.ResourceManager.GetString(CurrentShield.Id, new CultureInfo("en-US"));
            var LocalizedName = ShieldResources.ResourceManager.GetString(CurrentShield.Id);

            if (string.Compare(originalName, ShieldName, StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                MessengerInstance.Send<string>("goback", "navigation");
                CurrentShield.IsValidated = true;
            }
            else
            {
                MessageBox.Show(AppResources.Wrong);
            }
        }
    }
}

