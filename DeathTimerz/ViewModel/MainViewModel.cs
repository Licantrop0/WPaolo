using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows.Threading;
using DeathTimerz.Localization;

namespace DeathTimerz.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        string[] Advices;

        public MainViewModel()
        {
            InitializeTimer();
            Advices = HealthAdvices.ResourceManager
                .GetResourceSet(CultureInfo.CurrentCulture, true, true)
                .Cast<DictionaryEntry>()
                .Select(item => item.Value.ToString())
                .ToArray();
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
            }
        }

        public DateTime BirthDay
        {
            get { return Settings.BirthDay; }
            set
            {
                if (BirthDay == value) return;

                Settings.BirthDay = value;

                RaisePropertyChanged("BirthDay");
                RaisePropertyChanged("DaysToBirthDay");
                RaisePropertyChanged("AgeText");
            }
        }

        public string DaysToBirthDay
        {
            get
            {
                var dtb = ExtensionMethods.GetNextBirthday(BirthDay).Subtract(DateTime.Now).Days;
                return string.Join(" ", AppResources.BirthdayAfter,
                    dtb.ToString(),
                    dtb == 1 ? AppResources.Day : AppResources.Days);
            }
        }

        public string AgeText
        {
            get
            {
                //TODO: migliorare algoritmo
                var Age = DateTime.Now.Subtract(BirthDay);

                var Years = Math.Floor(Age.TotalDays / Settings.AverageYear);
                var RemainingDays = Age.TotalDays - Years * Settings.AverageYear;
                var Months = Math.Floor(RemainingDays / Settings.AverageMonth);
                var Days = Math.Round(RemainingDays - Months * Settings.AverageMonth);

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
                if (!Settings.EstimatedDeathAge.HasValue) return string.Empty;

                var EstimatedDeathDate = BirthDay.Add(Settings.EstimatedDeathAge.Value);

                if (EstimatedDeathDate > DateTime.Now)
                {
                    var TotalDaysLived = (DateTime.Now - BirthDay).TotalDays;
                    var TotalLifeDays = (EstimatedDeathDate - BirthDay).TotalDays;
                    return string.Format(AppResources.WillDie,
                        EstimatedDeathDate,
                        Settings.EstimatedDeathAge.Value.TotalDays / Settings.AverageYear,
                        TotalDaysLived / TotalLifeDays * 100,
                        TotalLifeDays - TotalDaysLived);
                }
                else
                    return AppResources.YetAlive;
            }
        }

        public string SuggestionText
        {
            get
            {
                return Advices[DateTime.Today.DayOfYear % Advices.Length];
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
