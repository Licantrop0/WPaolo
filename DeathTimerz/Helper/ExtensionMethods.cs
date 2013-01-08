using System;
using System.Linq;
using System.Collections.Generic;

namespace DeathTimerz
{
    public static class ExtensionMethods
    {
        public static TimeSpan Divide(this TimeSpan duration, int value)
        {
            return TimeSpan.FromTicks(duration.Ticks / value);
        }

        public static bool IsBirthDayThisYear(DateTime birthDay)
        {
            return new DateTime(DateTime.Now.Year, birthDay.Month, birthDay.Day,
                birthDay.Hour, birthDay.Minute, birthDay.Second) >= DateTime.Now;
        }

        public static DateTime GetNextBirthday(DateTime birthDay)
        {
            return IsBirthDayThisYear(birthDay) ?
                birthDay.AddYears(DateTime.Today.Year - birthDay.Year) :
                birthDay.AddYears(DateTime.Today.Year - birthDay.Year + 1);
        }

        public static bool SameBirthDay(this DateTime date1, DateTime date2)
        {
            return date1.Day == date2.Day &&
                date1.Month == date2.Month;        
        }

        public static TimeSpan TimeSpanFromYears(double year)
        {
            return TimeSpan.FromDays(year * AppContext.AverageYear);
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }
    }
}