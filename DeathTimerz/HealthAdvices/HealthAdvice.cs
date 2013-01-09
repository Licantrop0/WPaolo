using System;
using System.Collections;
using System.Globalization;
using System.Linq;

namespace HealthAdvices
{
    public static class HealthAdvice
    {
        private static string[] _advices;
        private static string[] Advices
        {
            get
            {
                if (_advices == null)
                    _advices = HealthAdvices.ResourceManager
                        .GetResourceSet(CultureInfo.CurrentCulture, true, true)
                        .Cast<DictionaryEntry>()
                        .Select(item => item.Value.ToString())
                        .ToArray();

                return _advices;
            }
        }

        public static string GetAdviceOfTheDay()
        {
            //var rnd = new Random();
            //var advice = Advices[rnd.Next(Advices.Length)];
            return Advices[DateTime.Today.DayOfYear % Advices.Length];
        }


    }
}
