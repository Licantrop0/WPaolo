using Scudetti.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using SocceramaWin8.Sound;
using Topics.Radical.Messaging;
using Topics.Radical.Windows.Presentation;
using System.Windows.Input;
using Topics.Radical.Windows.Input;
using Topics.Radical.Windows.Presentation.ComponentModel;

namespace SocceramaWin8.Presentation
{
    public class LevelViewModel : AbstractViewModel
    {
        ResourceLoader resources = new ResourceLoader();

        public int Number { get; private set; }
        public bool IsBonus { get; set; }
        public IEnumerable<Shield> Shields { get; private set; }
        public int TotalShields { get { return Shields.Count(); } }
        public int CompletedShields { get { return Shields.Count(s => s.IsValidated); } }

        public Thickness Margin
        {
            get
            {
                switch (Number)
                {
                    case 1:
                        return new Thickness(75, 440, 0, 0);
                    case 2:
                        return new Thickness(260, 200, 0, 0);
                    case 3:
                        return new Thickness(260, 680, 0, 0);
                    case 4:
                        return new Thickness(510, 440, 0, 0);
                    case 5:
                        return new Thickness(740, 260, 0, 0);
                    case 6:
                        return new Thickness(740, 620, 0, 0);
                    case 100:
                        return new Thickness(470, 870, 0, 0);
                    case 200:
                        return new Thickness(850, 40, 0, 0);
                    case 7:
                        return new Thickness(980, 260, 0, 0);
                    case 8:
                        return new Thickness(980, 620, 0, 0);
                    case 9:
                        return new Thickness(1210, 440, 0, 0);
                    case 10:
                        return new Thickness(1460, 200, 0, 0);
                    case 11:
                        return new Thickness(1460, 680, 0, 0);
                    case 12:
                        return new Thickness(1645, 440, 0, 0);
                    case 300:
                        return new Thickness(1250, 10, 0, 0);
                    default:
                        return new Thickness();
                }
            }
        }
        public bool IsUnlocked
        {
            get
            {
                if (IsBonus)
                    return AppContext.TotalShieldUnlocked >= Number / 100 * AppContext.BonusTreshold;
                else
                    return Number == 1 || AppContext.TotalShieldUnlocked >= (Number - 1) * AppContext.LockTreshold;
            }
        }

        public Uri Image
        {
            get
            {
                return IsUnlocked ?
                    new Uri("ms-appx:/Assets/Levels/livello" + Number + ".png") :
                    new Uri("ms-appx:/Assets/Levels/livello" + Number + "lock.png");
            }
        }

        public double Opacity
        {
            get { return IsUnlocked ? 1 : 0.6; }
        }

        public string Name
        {
            get
            {
                return IsBonus ?
                    string.Format("{0} {1}", resources.GetString("BonusLevel"), Number / 100) :
                    string.Format("{0} {1}", resources.GetString("Level"), Number);
            }
        }

        public string StatusText
        {
            get
            {
                if (IsUnlocked)
                {
                    return CompletedShields / TotalShields == 1 ? "*" :
                        string.Format("{0}/{1}", CompletedShields, TotalShields);
                }
                else
                {
                    return IsBonus ?
                        (AppContext.BonusTreshold * (Number / 100) - AppContext.TotalShieldUnlocked).ToString() :
                        (AppContext.LockTreshold * (Number - 1) - AppContext.TotalShieldUnlocked).ToString();
                }
            }
        }

        public LevelViewModel(IGrouping<int, Shield> group)
        {
            Number = group.Key;
            IsBonus = group.Key >= 100;
            Shields = group;

            //MessengerInstance.Register<PropertyChangedMessage<bool>>(this, (m) =>
            //{
            //    if (m.PropertyName != "IsValidated") return;

            //    RaisePropertyChanged("CompletedShields");
            //    RaisePropertyChanged("IsUnlocked");
            //    RaisePropertyChanged("StatusText");
            //    RaisePropertyChanged("LevelImage");
            //});
        }

        //private RelayCommand _resetCommand;
        //public RelayCommand ResetCommand
        //{
        //    get
        //    {
        //        return _resetCommand ?? (_resetCommand = new RelayCommand(async () =>
        //            {
        //                var d = new MessageDialog("Vuoi Cancellare tutto?"); //AppResources.ConfirmResetTitle);
        //                d.Commands.Add(new UICommand("Yes", ResetAction));
        //                d.Commands.Add(new UICommand("No"));
        //                await d.ShowAsync();
        //            }));
        //    }
        //}


        private ICommand _goToLevelCommand;
        public ICommand GoToLevelCommand
        {
            get
            {
                return _goToLevelCommand ?? (_goToLevelCommand =
                    DelegateCommand.Create().OnExecute(o =>
                    {
                        if (!this.IsUnlocked) return;
                        SoundManager.PlayFischietto();
                        ns.Navigate<ShieldsView>(this);
                    }));
            }
        }
    }
}
