using DeathTimerz.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathTimerz.ViewModel
{
    public class EpighraphViewModel
    {
        private DateTime BirthDay
        {
            get
            {
                return AppContext.BirthDay.Value;
            }
        }

        public string EstimatedDeathAgeText
        {
            get
            {
                if (!AppContext.TimeToDeath.HasValue) return string.Empty;

                var EstimateDeathAge = AppContext.TimeToDeath.Value +
                    ExtensionMethods.TimeSpanFromYears(AppContext.AverageAge);

                var EstimatedDeathDate = BirthDay + EstimateDeathAge;

                if (EstimatedDeathDate > DateTime.Now)
                {
                    var TotalDaysLived = (DateTime.Now - BirthDay).TotalDays;
                    var TotalLifeDays = (EstimatedDeathDate - BirthDay).TotalDays;
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

    }
}
