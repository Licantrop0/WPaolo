using System;
using System.Collections;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using DeathTimerz.Localization;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Shell;
using DeathTimerz.Helper;
using System.Windows.Media.Imaging;

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

        public Visibility IsPinSuggestionVisible
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("is_pin_suggestion_visible"))
                    IsolatedStorageSettings.ApplicationSettings["is_pin_suggestion_visible"] = true;
                return (bool)IsolatedStorageSettings.ApplicationSettings["is_pin_suggestion_visible"] ?
                    Visibility.Visible : Visibility.Collapsed;
            }
            set
            {
                if (IsPinSuggestionVisible == value) return;
                IsolatedStorageSettings.ApplicationSettings["is_pin_suggestion_visible"] =
                    (value == Visibility.Visible);
                RaisePropertyChanged("IsPinSuggestionVisible");
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

        //public Visibility BirthdayCakeVisibility
        //{
        //    get
        //    {
        //        if (!BirthDayInserted)
        //            return Visibility.Collapsed;

        //        return AppContext.BirthDay.Value.SameBirthDay(DateTime.Today) ?
        //            Visibility.Visible : Visibility.Collapsed;
        //    }
        //}

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

                var dtb = ExtensionMethods.GetNextBirthday(BirthDay.Value).Subtract(DateTime.Now).Days;
                return string.Join(" ", AppResources.BirthdayAfter,
                    dtb.ToString(),
                    dtb == 1 ? AppResources.Day : AppResources.Days);
            }
        }

        public string AgeText
        {
            get
            {
                if (!BirthDayInserted) return AppResources.InsertBirthday;

                //TODO: migliorare algoritmo
                var Age = DateTime.Now.Subtract(BirthDay.Value);

                var Years = Math.Floor(Age.TotalDays / AppContext.AverageYear);
                var RemainingDays = Age.TotalDays - Years * AppContext.AverageYear;
                var Months = Math.Floor(RemainingDays / AppContext.AverageMonth);
                var Days = Math.Round(RemainingDays - Months * AppContext.AverageMonth);

                return
                    Years.ToString("#0") + " " + (Years == 1 ? AppResources.Year : AppResources.Years) + "\n" +
                    Months.ToString("0") + " " + (Months == 1 ? AppResources.Month : AppResources.Months) + "\n" +
                    Days.ToString("0") + " " + (Days == 1 ? AppResources.Day : AppResources.Days) + "\n" +
                    Age.Hours.ToString("0") + " " + (Age.Hours == 1 ? AppResources.Hour : AppResources.Hours) + "\n" +
                    Age.Minutes.ToString("0") + " " + (Age.Minutes == 1 ? AppResources.Minute : AppResources.Minutes);
            }
        }

        public string EstimatedDeathAgeText
        {
            get
            {
                if (!BirthDayInserted) return AppResources.InsertBirthday;
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
