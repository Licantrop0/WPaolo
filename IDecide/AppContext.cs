using System.Collections.Generic;
using System.IO.IsolatedStorage;
using IDecide.Localization;
using OcKvp = System.Collections.ObjectModel.ObservableCollection<System.Collections.Generic.KeyValuePair<string, string>>;

namespace IDecide
{
    public class AppContext
    {
        public static OcKvp ChoicesGroup
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("choices_group"))
                    IsolatedStorageSettings.ApplicationSettings["choices_group"] = GetDefaultChoices();
                return (OcKvp)IsolatedStorageSettings.ApplicationSettings["choices_group"];
            }
            set
            {
                if (ChoicesGroup != value)
                    IsolatedStorageSettings.ApplicationSettings["choices_group"] = value;
            }
        }


        public static string SelectedGroup
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("selected_group"))
                    IsolatedStorageSettings.ApplicationSettings["selected_group"] = "MagicBall";
                return (string)IsolatedStorageSettings.ApplicationSettings["selected_group"];
            }
            set
            {
                if (SelectedGroup != value)
                    IsolatedStorageSettings.ApplicationSettings["selected_group"] = value;
            }
        }

        private static OcKvp GetDefaultChoices()
        {
            var Choices = new OcKvp();

            Choices.Add(new KeyValuePair<string, string>("YesNoMaybe", AppResources.Yes));
            Choices.Add(new KeyValuePair<string, string>("YesNoMaybe", AppResources.No));
            Choices.Add(new KeyValuePair<string, string>("YesNoMaybe", AppResources.Maybe));

            Choices.Add(new KeyValuePair<string, string>("HeadTail", AppResources.Head));
            Choices.Add(new KeyValuePair<string, string>("HeadTail", AppResources.Tail));

            Choices.Add(new KeyValuePair<string, string>("RPSLS", AppResources.Rock));
            Choices.Add(new KeyValuePair<string, string>("RPSLS", AppResources.Paper));
            Choices.Add(new KeyValuePair<string, string>("RPSLS", AppResources.Scissor));
            Choices.Add(new KeyValuePair<string, string>("RPSLS", AppResources.Lizard));
            Choices.Add(new KeyValuePair<string, string>("RPSLS", AppResources.Spock));

            for (int i = 1; i <= 20; i++)
                Choices.Add(new KeyValuePair<string, string>("MagicBall",
                    AppResources.ResourceManager.GetString("MagicBall" + i)));

            for (int i = 0; i <= 100; i++)
                Choices.Add(new KeyValuePair<string, string>("Percentage", i + "%"));

            return Choices;
        }
    }
}
