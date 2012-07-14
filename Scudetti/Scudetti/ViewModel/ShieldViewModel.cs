using System;
using System.Globalization;
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
        public string InputShieldName { get; set; }
        public Visibility InputVisibile { get { return CurrentShield.IsValidated ? Visibility.Collapsed : Visibility.Visible; } }
        public string OriginalName { get { return ShieldResources.ResourceManager.GetString(CurrentShield.Id, new CultureInfo("en-US")); } }
        
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

        public ShieldViewModel()
        {
            if (IsInDesignMode)
            {
                CurrentShield = DesignTimeData.Shields.First();
            }
            InputShieldName = string.Empty;
        }

        public void Validate()
        {
            //TODO: Fare il compare sia con il nome originale che con il nome localizzato
            var LocalizedName = ShieldResources.ResourceManager.GetString(CurrentShield.Id);

            if (string.Compare(OriginalName, InputShieldName, StringComparison.InvariantCultureIgnoreCase) == 0)
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

