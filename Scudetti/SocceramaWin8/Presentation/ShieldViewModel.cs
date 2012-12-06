using GalaSoft.MvvmLight.Messaging;
using Scudetti.Model;
using System.Linq;
using Topics.Radical.ComponentModel.Messaging;
using Topics.Radical.Windows.Presentation;
using Topics.Radical.Windows.Presentation.ComponentModel;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Topics.Radical.ComponentModel.Windows.Input;
using Topics.Radical.Windows.Input;
using SocceramaWin8.Sound;
using System;
using SocceramaWin8.Helpers;

namespace SocceramaWin8.Presentation
{
    public class ShieldViewModel : AbstractViewModel, IExpectNavigatingToCallback
    {
        ResourceLoader resources = new ResourceLoader();
        readonly INavigationService _ns;
        readonly IMessageBroker _broker;
        public IDelegateCommand GoBack { get; private set; }

        public Visibility InputVisibile { get { return CurrentShield.IsValidated ? Visibility.Collapsed : Visibility.Visible; } }

        public string ShieldName
        {
            get
            {
                return CurrentShield.IsValidated ?
                    CurrentShield.Names.First() :
                    "Inserisci il nome della squadra";
            }
        }

        private string _hintText;
        public string HintText
        {
            get
            {
                return _hintText ?? (_hintText = string.Format(resources.GetString("AvailableHints"), AppContext.AvailableHints));
            }
            set
            {
                _hintText = value;
                OnPropertyChanged("HintText");
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
                        _broker.Broadcast(this, new PropertyChangedMessage<bool>(
                        !_currentShield.IsValidated,
                        _currentShield.IsValidated,
                        e.PropertyName));
                    }
                };
                OnPropertyChanged("CurrentShield");

            }
        }

        public bool ControlsEnabled { get { return !CurrentShield.IsValidated; } }

        public ShieldViewModel(INavigationService ns, IMessageBroker broker)
        {
            _ns = ns;
            _broker = broker;

            this.CurrentShield = new Shield();

            GoBack = DelegateCommand
                .Create()
                .OnExecute((obj) => _ns.GoBack());

            //if (isIndesignMode)
            //{
            //    _currentShield = DesignTimeData.Shields.First();
            //    _hintText = string.Format(resources.GetString("AvailableHints"), 5);
            //    return;
            //}
        }

        void IExpectNavigatingToCallback.OnNavigatingTo(NavigationEventArgs e)
        {
            CurrentShield = (Shield)e.Arguments;
            InputShieldName = string.Empty;
            HintText = null;
            _hintUsed = false;
            OnPropertyChanged("InputVisibile");
            OnPropertyChanged("ShieldName");
        }

        private bool _hintUsed = false;
        private bool HintEnabled(object arg)
        {
            return AppContext.AvailableHints > 0 && !_hintUsed;
        }

        public string InputShieldName { get; set; }


        #region Commands

        private IDelegateCommand _showHintCommand;
        public IDelegateCommand ShowHintCommand
        {
            get
            {
                return _showHintCommand ?? (_showHintCommand =
                    DelegateCommand.Create()
                    .OnExecute(ShowHint)
                    .OnCanExecute(HintEnabled));
            }
        }

        private void ShowHint(object o)
        {
            if (AppContext.AvailableHints > 0)
            {
                HintText = CurrentShield.Hint;
                AppContext.AvailableHints--;
                _hintUsed = true;
                ShowHintCommand.EvaluateCanExecute();
            }
            else
            {
                //MessageBox.Show(resources.GetString("NoHintsAvailable"));
            }
        }


        public bool Validate()
        {
            if (CurrentShield.IsValidated || string.IsNullOrEmpty(InputShieldName))
            {
                _ns.GoBack();
            }
            else if (CurrentShield.Names.Any(name => CompareName(name, InputShieldName)))
            {
                SoundManager.PlayValidated();
                CurrentShield.IsValidated = true;
                if (AppContext.TotalShieldUnlocked % AppContext.HintsTreshold == 0)
                    AppContext.AvailableHints++;

                ShowNotifications();

                _ns.GoBack();
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

        private void ShowNotifications()
        {
            //e ho già sbloccato degli scudetti
            if (AppContext.TotalShieldUnlocked == 0) return;

            var newLevelUnlocked = AppContext.TotalShieldUnlocked % AppContext.LockTreshold == 0;
            var newBonusLevelUnlocked = AppContext.TotalShieldUnlocked % AppContext.BonusTreshold == 0;

            if (newLevelUnlocked)
            {
                int newLevelNumber = (AppContext.TotalShieldUnlocked / AppContext.LockTreshold) + 1;
                if (newLevelNumber <= 6)
                {
                    var Message = string.Format(resources.GetString("NewLevel"), newLevelNumber);
                    NotificationHelper.DisplayToast(Message);
                }
            }
            else if (newBonusLevelUnlocked)
            {
                int newBonusLevelNumber = ((AppContext.TotalShieldUnlocked / AppContext.BonusTreshold));
                var Message = string.Format(resources.GetString("NewBonusLevel"), newBonusLevelNumber);
                NotificationHelper.DisplayToast(Message);
            }
            else if (AppContext.GameCompleted) //Gioco completato!
            {
                //SoundManager.PlayGoal();
                NotificationHelper.DisplayToast(resources.GetString("GameFinished"));
                //Title = AppResources.GameFinishedTitle,
                //Message = AppResources.GameFinished,
                //TextWrapping = TextWrapping.Wrap,
                //MillisecondsUntilHidden = 8000,
            }
        }

        #endregion

    }
}
