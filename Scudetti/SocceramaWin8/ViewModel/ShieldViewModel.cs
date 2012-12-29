using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Scudetti.Model;
using SocceramaWin8.Data;
using SocceramaWin8.Helpers;
using SocceramaWin8.Sound;
using System;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;

namespace SocceramaWin8.ViewModel
{
    public class ShieldViewModel : ViewModelBase
    {
        ResourceLoader resources = new ResourceLoader();
        public string InputShieldName { get; set; }
        public Visibility InputVisibile { get { return CurrentShield.IsValidated ? Visibility.Collapsed : Visibility.Visible; } }
        public string Title
        {
            get
            {
                return CurrentShield.IsValidated ?
                    resources.GetString("GoBack") :
                    resources.GetString("InsertTeam");
            }
        }

        private string _hintText;
        public string HintText
        {
            get
            {
                if (CurrentShield.IsValidated) return CurrentShield.Names.First();
                return _hintText ?? (_hintText = string.Format(
                    resources.GetString("AvailableHints"),
                    AppContext.AvailableHints));
            }
            set
            {
                _hintText = value;
                RaisePropertyChanged("HintText");
            }
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

        public bool ControlsEnabled { get { return !CurrentShield.IsValidated; } }

        public ShieldViewModel()
        {
            if (IsInDesignMode)
            {
                _currentShield = DesignTimeData.Shields.First();
                _hintText = string.Format(resources.GetString("AvailableHints"), 5);
                return;
            }
            MessengerInstance.Register<Shield>(this, shield =>
            {
                CurrentShield = shield;
                InputShieldName = string.Empty;
                HintText = null;
                _hintUsed = false;
            });
        }

        #region Commands

        private RelayCommand _showHintCommand;
        public RelayCommand ShowHintCommand
        {
            get
            {
                return _showHintCommand ?? (_showHintCommand =
                    new RelayCommand(ShowHint, HintEnabled));
            }
        }

        private void ShowHint()
        {
            if (AppContext.AvailableHints > 0)
            {
                HintText = CurrentShield.Hint;
                AppContext.AvailableHints--;
                _hintUsed = true;
                ShowHintCommand.RaiseCanExecuteChanged();
            }
            else
            {
                HintText = resources.GetString("NoHintsAvailable");
            }
        }

        private bool _hintUsed = false;
        private bool HintEnabled()
        {
            return AppContext.AvailableHints > 0 && !_hintUsed;
        }

        public bool Validate()
        {
            if (CurrentShield.IsValidated || string.IsNullOrEmpty(InputShieldName))
            {
                MessengerInstance.Send("goback", "navigation");
            }
            else if (CurrentShield.Names.Any(name => CompareName(name, InputShieldName)))
            {
                SoundManager.PlayValidated();
                CurrentShield.IsValidated = true;

                if (AppContext.TotalShieldUnlocked % AppContext.HintsTreshold == 0)
                    AppContext.AvailableHints++;

                MessengerInstance.Send("goback", "navigation");
            }
            else
            {
                SoundManager.PlayBooh();
                //MessageBox.Show(AppResources.Wrong);
                return false;
            }

            return true;
        }

        private bool CompareName(string string1, string string2)
        {
            return string.Compare(string1, string2.Trim(), StringComparison.CurrentCultureIgnoreCase) == 0;
        }

        #endregion

    }
}
