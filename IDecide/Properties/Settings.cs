using System;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace IDecide
{
    public static class Settings
    {
        public static ObservableCollection<string> Choices
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("choices"))
                    IsolatedStorageSettings.ApplicationSettings["choices"] = new ObservableCollection<string>();
                return (ObservableCollection<string>)IsolatedStorageSettings.ApplicationSettings["choices"];
            }
            set
            {
                if (Choices != value)
                    IsolatedStorageSettings.ApplicationSettings["choices"] = value;
            }
        }

        public static IEnumerable<string> GetYesNoMaybe()
        {
            yield return AppResources.Yes;
            yield return AppResources.No;
            yield return AppResources.Maybe;
        }

        public static IEnumerable<string> GetHeadOrTail()
        {
            yield return AppResources.Head;
            yield return AppResources.Tail;
        }

        public static IEnumerable<string> GetMagicBall()
        {
            for (int i = 1; i <= 20; i++)
                yield return AppResources.ResourceManager.GetString("MagicBall" + i);
        }

        public static IEnumerable<string> GetPercentage()
        {
            return Enumerable.Range(0, 101).Select(i => i + "%");
        }

        public static IEnumerable<string> GetRPSLS()
        {
            yield return AppResources.Rock;
            yield return AppResources.Paper;
            yield return AppResources.Scissor;
            yield return AppResources.Lizard;
            yield return AppResources.Spock;
        }


    }
}