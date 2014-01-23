using DeathTimerz.Localization;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Shell;
using System;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace DeathTimerz.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            InitializeTimer();
            Messenger.Default.Register<NotificationMessage>(this,
                m => { if (m.Notification == "TestUpdated") UpdateTest(); });
        }

        private void UpdateTest()
        {
            RaisePropertyChanged("TestButtonVisibility");
            RaisePropertyChanged("EstimatedDeathAgeText");
            RaisePropertyChanged("TombStoneVisibility");
        }

        private void InitializeTimer()
        {
            var dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromMinutes(1);
            dt.Tick += (sender, e) => RaisePropertyChanged("AgeText");
            dt.Start();
        }

        public bool IsMale
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("is_male"))
                    IsolatedStorageSettings.ApplicationSettings["is_male"] = true;
                return (bool)IsolatedStorageSettings.ApplicationSettings["is_male"];
            }
            set
            {
                if (IsMale == value) return;
                IsolatedStorageSettings.ApplicationSettings["is_male"] = value;
                RaisePropertyChanged("IsMale");
                RaisePropertyChanged("EstimatedDeathAgeText");
            }
        }

        public DateTime? BirthDay
        {
            get { return AppContext.BirthDay; }
            set
            {
                if (BirthDay == value) return;

                if (value.HasValue && CheckBirthday(value.Value))
                {
                    AppContext.BirthDay = value;
                    RaisePropertyChanged("DaysToBirthDay");
                    RaisePropertyChanged("AgeText");
                    RaisePropertyChanged("EstimatedDeathAgeText");
                    RaisePropertyChanged("TestButtonVisibility");
                    RaisePropertyChanged("InserBirthadyVisibility");
                }
                RaisePropertyChanged("BirthDay");
                RaisePropertyChanged("BirthDayInserted");
                RaisePropertyChanged("BirthdayCakeVisibility");
            }
        }

        public bool BirthDayInserted
        {
            get { return AppContext.BirthDay.HasValue; }
        }

        public Visibility InserBirthadyVisibility
        {
            get { return BirthDayInserted ? Visibility.Collapsed : Visibility.Visible; }
        }

        public Visibility PinSuggestionVisibility
        {
            get { return ShellTile.ActiveTiles.Count() == 1 && AppContext.PinSuggestionVisible?
                Visibility.Visible : Visibility.Collapsed; }
        }

        private RelayCommand _closePinSuggestionCommand;
        public RelayCommand ClosePinSuggestionCommand
        {
            get
            {
                return _closePinSuggestionCommand ?? (_closePinSuggestionCommand = new RelayCommand(() =>
                {
                    AppContext.PinSuggestionVisible = false;
                    RaisePropertyChanged("PinSuggestionVisibility");
                }));
            }
        }

        public bool CheckBirthday(DateTime value)
        {
            if (value >= DateTime.Today)
            {
                MessageBox.Show(AppResources.ErrorFutureBirthday);
                return false;
            }

            //Trick per evitare il bug del DatePicker quando si imposta 1600 come anno
            if (value < DateTime.Now.AddYears(-130))
            {
                MessageBox.Show(AppResources.ErrorTooOldBirthday);
                return false;
            }

            return true;
        }

        public string DaysToBirthDay
        {
            get
            {
                if (!BirthDayInserted) return string.Empty;

                var dtb = ExtensionMethods.GetNextBirthday(BirthDay.Value).Subtract(DateTime.Today).Days;
                if (dtb == 0) return AppResources.BirthdayToday;
                else if (dtb == 1) return string.Format(AppResources.BirthdayAfter, dtb, AppResources.Day);
                else return string.Format(AppResources.BirthdayAfter, dtb, AppResources.Days);
            }
        }

        public string AgeText
        {
            get
            {
                if (!BirthDayInserted) return string.Empty;

                //TODO: migliorare algoritmo
                var Age = DateTime.Now.Subtract(BirthDay.Value);
                var Years = Math.Floor(Age.TotalDays / AppContext.AverageYear);
                var RemainingDays = Age.TotalDays - Years * AppContext.AverageYear;
                var Months = Math.Floor(RemainingDays / AppContext.AverageMonth);
                var Days = Math.Round(RemainingDays - Months * AppContext.AverageMonth);
                var nbsp = Convert.ToChar(160);

                var AgeSB = new StringBuilder(AppResources.Age);
                AgeSB.AppendFormat(": {0:#0}{1}{2}, ", Years, nbsp,
                    Years == 1 ? AppResources.Year : AppResources.Years);
                AgeSB.AppendFormat("{0:#0}{1}{2}, ", Months, nbsp,
                    Months == 1 ? AppResources.Month : AppResources.Months);
                AgeSB.AppendFormat("{0:#0}{1}{2}, ", Days, nbsp,
                    Days == 1 ? AppResources.Day : AppResources.Days);
                AgeSB.AppendFormat("{0:#0}{1}{2}, ", Age.Hours, nbsp,
                    Age.Hours == 1 ? AppResources.Hour : AppResources.Hours);
                AgeSB.AppendFormat("{0:#0}{1}{2}", Age.Minutes, nbsp,
                    Age.Minutes == 1 ? AppResources.Minute : AppResources.Minutes);

                return AgeSB.ToString();
            }
        }

        public Visibility TombStoneVisibility
        {
            get
            {
                return string.IsNullOrEmpty(EstimatedDeathAgeText) ?
                    Visibility.Collapsed :
                    Visibility.Visible;
            }
        }

        public string EstimatedDeathAgeText
        {
            get
            {
                if (!BirthDay.HasValue) return string.Empty;
                if (!AppContext.TimeToDeath.HasValue) return string.Empty;

                var EstimateDeathAge = AppContext.TimeToDeath.Value +
                    ExtensionMethods.TimeSpanFromYears(AppContext.AverageAge);

                var EstimatedDeathDate = BirthDay.Value + EstimateDeathAge;

                if (EstimatedDeathDate > DateTime.Now)
                {
                    var TotalDaysLived = (DateTime.Now - BirthDay.Value).TotalDays;
                    var TotalLifeDays = (EstimatedDeathDate - BirthDay.Value).TotalDays;
                    return string.Format(AppResources.WillDie,
                        EstimatedDeathDate,
                        EstimateDeathAge.TotalDays / AppContext.AverageYear,
                        TotalDaysLived / TotalLifeDays * 100,
                        TotalLifeDays - TotalDaysLived);
                }
                else
                    return AppResources.YetAlive;
            }
        }

        public Visibility TestButtonVisibility
        {
            get
            {
                return AppContext.TimeToDeath.HasValue ||
                    !AppContext.BirthDay.HasValue ?
                    Visibility.Collapsed : Visibility.Visible;
            }
        }

        public string SuggestionText
        {
            get
            {
                return HealthAdvices.HealthAdvice.GetAdviceOfTheDay();
            }
        }
    }
}
