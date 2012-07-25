using System;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Scudetti.Data;
using Scudetti.Localization;
using Scudetti.Model;

namespace Scudetti.ViewModel
{
    public class ShieldViewModel : ViewModelBase
    {
        public string InputShieldName { get; set; }
        public Visibility InputVisibile { get { return CurrentShield.IsValidated ? Visibility.Collapsed : Visibility.Visible; } }
        public string OriginalName { get { return CurrentShield.Names[0]; } }
        public int AvailableHints
        {
            get { return AppContext.AvailableHints; }
            set
            {
                AppContext.AvailableHints = value;
                RaisePropertyChanged("AvailableHintsText");
            }
        }

        public string AvailableHintsText
        {
            get { return string.Format(AppResources.AvailableHints, AvailableHints); }
        }

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
                        MessengerInstance.Send(new PropertyChangedMessage<bool>(
                            !_currentShield.IsValidated,
                            _currentShield.IsValidated,
                            e.PropertyName));
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
            if (CurrentShield.IsValidated || string.IsNullOrEmpty(InputShieldName))
            {
                MessengerInstance.Send("goback", "navigation");
            }
            else if (CurrentShield.Names.Any(name => CompareName(name, InputShieldName)))
            {
                CurrentShield.IsValidated = true;
                if (AppContext.TotalShieldUnlocked % 5 == 0)
                    AvailableHints++;

                MessengerInstance.Send("goback", "navigation");
            }
            else
            {
                MessageBox.Show(AppResources.Wrong);
            }
        }

        private bool CompareName(string string1, string string2)
        {
            return string.Compare(string1, string2, StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        public void ShowHint()
        {
            if (AvailableHints > 0)
            {
                MessageBox.Show(CurrentShield.Hint);
                AvailableHints--;
            }
            else
            {
                MessageBox.Show(AppResources.NoHintsAvailable);
            }
        }
    }
}

