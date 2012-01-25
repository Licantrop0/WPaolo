using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using IDecide.Localization;
using IDecide.ViewModel;
using System;
using System.IO.IsolatedStorage;

namespace IDecide
{
    public static class AppContext
    {
        public static ObservableCollection<ChoiceGroupViewModel> Groups { get; set; }
        public static bool SoundEnabled { get; set; }
        public static bool VibrationEnabled { get; set; }

        public static string GetRandomChoice()
        {
            if (!Groups.Any())
                return AppResources.NothingToDecide;

            var selectedGroup =  Groups
                .Where(c => c.IsSelected);

            if(!selectedGroup.Any())
                return AppResources.NothingToDecide;

            var selectedChoices = selectedGroup
                .Single().Choices.ToList();

            if (!selectedChoices.Any())
                return AppResources.NothingToDecide;

            var rnd = new Random();
            return selectedChoices[rnd.Next(selectedChoices.Count)];
                
        }

        internal static void LoadData()
        {
            if (!IsolatedStorageSettings.ApplicationSettings.Contains("choices_group"))
                IsolatedStorageSettings.ApplicationSettings.Add("choices_group", new ObservableCollection<ChoiceGroupViewModel>());

            var groups = (ObservableCollection<ChoiceGroupViewModel>)IsolatedStorageSettings.ApplicationSettings["choices_group"];
            if (groups.Any())
                Groups = groups;
            else
                Groups = GetDefaultChoices();

            if (!IsolatedStorageSettings.ApplicationSettings.Contains("sound_enabled"))
                IsolatedStorageSettings.ApplicationSettings.Add("sound_enabled", true);
            SoundEnabled = (bool)IsolatedStorageSettings.ApplicationSettings["sound_enabled"];

            if (!IsolatedStorageSettings.ApplicationSettings.Contains("vibration_enabled"))
                IsolatedStorageSettings.ApplicationSettings.Add("vibration_enabled", true);
            VibrationEnabled = (bool)IsolatedStorageSettings.ApplicationSettings["vibration_enabled"];
        }

        internal static void PersistData()
        {
            IsolatedStorageSettings.ApplicationSettings["choices_group"] = Groups;
            IsolatedStorageSettings.ApplicationSettings["sound_enabled"] = SoundEnabled;
            IsolatedStorageSettings.ApplicationSettings["vibration_enabled"] = VibrationEnabled;
        }


        public static ObservableCollection<ChoiceGroupViewModel> GetDefaultChoices()
        {
            var Choices = new ObservableCollection<ChoiceGroupViewModel>();

            Choices.Add(new ChoiceGroupViewModel()
            {
                Name = "MagicBall",
                IsSelected = true,
                Choices = new ObservableCollection<string>(
                    Enumerable.Range(1, 20).Select(i =>
                    DefaultChoices.ResourceManager.GetString("MagicBall" + i)))
            });

            Choices.Add(new ChoiceGroupViewModel()
            {
                Name = "Dice",
                Choices = new ObservableCollection<string>(
                    Enumerable.Range(1, 6).Select(i =>
                    DefaultChoices.ResourceManager.GetString("Dice" + i)))
            });


            Choices.Add(new ChoiceGroupViewModel()
            {
                Name = "HeadTail",
                Choices = new ObservableCollection<string>(new[]
                {
                    DefaultChoices.Head,
                    DefaultChoices.Tail
                })
            });

            Choices.Add(new ChoiceGroupViewModel()
            {
                Name = "YesNoMaybe",
                Choices = new ObservableCollection<string>(new[]
                { 
                    DefaultChoices.Yes,
                    DefaultChoices.No,
                    DefaultChoices.Maybe
                })
            });

            Choices.Add(new ChoiceGroupViewModel()
            {
                Name = "RPSLS",
                Choices = new ObservableCollection<string>(new[]
                {
                    DefaultChoices.Rock, DefaultChoices.Paper, DefaultChoices.Scissor,
                    DefaultChoices.Lizard, DefaultChoices.Spock
                })
            });

            Choices.Add(new ChoiceGroupViewModel()
            {
                Name = "Percentage",
                Choices = new ObservableCollection<string>(
                    Enumerable.Range(0, 100).Select(i => i + "%"))
            });

            return Choices;
        }

    }
}