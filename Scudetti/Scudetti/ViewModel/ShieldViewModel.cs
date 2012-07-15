using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using Scudetti.Data;
using Scudetti.Localization;
using Scudetti.Model;
using GalaSoft.MvvmLight.Messaging;

namespace Scudetti.ViewModel
{
    public class ShieldViewModel : ViewModelBase
    {
        public string InputShieldName { get; set; }
        public Visibility InputVisibile { get { return CurrentShield.IsValidated ? Visibility.Collapsed : Visibility.Visible; } }
        public string OriginalName { get { return CurrentShield.Names[0]; } }

        private Shield _currentShield;
        public Shield CurrentShield
        {
            get { return _currentShield; }
            set
            {
                if (CurrentShield == value)
                    return;
                _currentShield = value;
                _currentShield.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == "IsValidated")
                    {
                        MessengerInstance.Send<PropertyChangedMessage<bool>>(
                            new PropertyChangedMessage<bool>(!_currentShield.IsValidated,
                                _currentShield.IsValidated, e.PropertyName));
                    }
                };
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
            //Fa il compare con tutti i vari nomi
            foreach (var name in CurrentShield.Names)
            {
                if (string.Compare(name, InputShieldName, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    CurrentShield.IsValidated = true;
                    MessengerInstance.Send<string>("goback", "navigation");
                    return;
                }
            }

            MessageBox.Show(AppResources.Wrong);
        }
    }
}

