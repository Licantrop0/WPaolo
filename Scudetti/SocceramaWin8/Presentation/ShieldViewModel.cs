using Scudetti.Model;
using SocceramaWin8.Data;
using SocceramaWin8.Helpers;
using SocceramaWin8.Sound;
using System;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;

namespace SocceramaWin8.Presentation
{
    public class ShieldViewModel
    {
        ResourceLoader resources = new ResourceLoader();
        public string InputShieldName { get; set; }
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
                //RaisePropertyChanged("HintText");
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

                //_currentShield.PropertyChanged += (sender, e) =>
                //{
                //    if (e.PropertyName == "IsValidated")
                //    {
                //        MessengerInstance.Send(new PropertyChangedMessage<bool>(
                //            !_currentShield.IsValidated,
                //            _currentShield.IsValidated,
                //            e.PropertyName));
                //    }
                //};
                //RaisePropertyChanged("CurrentShield");
            }
        }

        public bool ControlsEnabled { get { return !CurrentShield.IsValidated; } }

        public ShieldViewModel()
        {
            //if (IsInDesignMode)
            //{
            //    _currentShield = DesignTimeData.Shields.First();
            //    _hintText = string.Format(resources.GetString("AvailableHints"), 5);
            //    return;
            //}
            //MessengerInstance.Register<Shield>(this, shield =>
            //{
            //    CurrentShield = shield;
            //    InputShieldName = string.Empty;
            //    HintText = null;
            //    _hintUsed = false;
            //});
        }

        //#region Commands

        //private RelayCommand _showHintCommand;
        //public RelayCommand ShowHintCommand
        //{
        //    get
        //    {
        //        return _showHintCommand ?? (_showHintCommand =
        //            new RelayCommand(ShowHint, HintEnabled));
        //    }
        //}

        //private void ShowHint()
        //{
        //    if (AppContext.AvailableHints > 0)
        //    {
        //        HintText = CurrentShield.Hint;
        //        AppContext.AvailableHints--;
        //        _hintUsed = true;
        //        ShowHintCommand.RaiseCanExecuteChanged();
        //    }
        //    else
        //    {
        //        //MessageBox.Show(resources.GetString("NoHintsAvailable"));
        //    }
        //}

        //private bool _hintUsed = false;
        //private bool HintEnabled()
        //{
        //    return AppContext.AvailableHints > 0 && !_hintUsed;
        //}


        //public bool Validate()
        //{
        //    if (CurrentShield.IsValidated || string.IsNullOrEmpty(InputShieldName))
        //    {
        //        MessengerInstance.Send("goback", "navigation");
        //    }
        //    else if (CurrentShield.Names.Any(name => CompareName(name, InputShieldName)))
        //    {
        //        SoundManager.PlayValidated();
        //        CurrentShield.IsValidated = true;
        //        if (AppContext.TotalShieldUnlocked % AppContext.HintsTreshold == 0)
        //            AppContext.AvailableHints++;

        //        ShowNotifications();

        //        MessengerInstance.Send("goback", "navigation");
        //    }
        //    else
        //    {
        //        SoundManager.PlayBooh();
        //        //MessageBox.Show(AppResources.Wrong);
        //        return false;
        //    }

        //    return true;
        //}

        //private bool CompareName(string string1, string string2)
        //{
        //    return string.Compare(string1, string2.Trim(), StringComparison.CurrentCultureIgnoreCase) == 0;
        //}

        //private void ShowNotifications()
        //{
        //    //e ho già sbloccato degli scudetti
        //    if (AppContext.TotalShieldUnlocked == 0) return;

        //    var newLevelUnlocked = AppContext.TotalShieldUnlocked % AppContext.LockTreshold == 0;
        //    var newBonusLevelUnlocked = AppContext.TotalShieldUnlocked % AppContext.BonusTreshold == 0;

        //    if (newLevelUnlocked)
        //    {
        //        int newLevelNumber = (AppContext.TotalShieldUnlocked / AppContext.LockTreshold) + 1;
        //        if (newLevelNumber <= 6)
        //        {
        //            var level = AppContext.Levels.Single(l => l.Number == newLevelNumber);
        //            var Message = string.Format(resources.GetString("NewLevel"), level.Number);
        //            NotificationHelper.DisplayToast(Message);
        //        }
        //    }
        //    else if (newBonusLevelUnlocked)
        //    {
        //        int newLevelNumber = ((AppContext.TotalShieldUnlocked / AppContext.BonusTreshold)) * 100;
        //        var level = AppContext.Levels.Single(l => l.Number == newLevelNumber);
        //        var Message = string.Format(resources.GetString("NewLevel"), level.Number);
        //        NotificationHelper.DisplayToast(Message);
        //    }
        //    else if (AppContext.GameCompleted) //Gioco completato!
        //    {
        //        //SoundManager.PlayGoal();
        //        NotificationHelper.DisplayToast(resources.GetString("GameFinished"));
        //        //Title = AppResources.GameFinishedTitle,
        //        //Message = AppResources.GameFinished,
        //        //TextWrapping = TextWrapping.Wrap,
        //        //MillisecondsUntilHidden = 8000,
        //    }
        //}

        //#endregion

    }
}
