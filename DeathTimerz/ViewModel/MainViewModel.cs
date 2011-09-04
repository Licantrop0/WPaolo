using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using DeathTimerz.Localization;
using System.Windows.Threading;

namespace DeathTimerz.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private DispatcherTimer dt = new DispatcherTimer();

        public MainViewModel()
        {
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
                if (!CheckBirthday(value)) return;

                Settings.BirthDay = value;

                RaisePropertyChanged("BirthDay");
                RaisePropertyChanged("DaysToBirthDay");
                RaisePropertyChanged("AgeText");
            }
        }

        private bool CheckBirthday(DateTime value)
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
                BirthDay = DateTime.Now.AddYears(-50);
                return false;
            }

            return true;
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
                // + "\n" + Age.Seconds.ToString("0") + " " + (Age.Seconds == 1 ? AppResources.Second : AppResources.Seconds);
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
                return "Se sei scemo vivi di meno!";
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
