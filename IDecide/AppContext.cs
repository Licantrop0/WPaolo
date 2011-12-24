using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using IDecide.Localization;
using IDecide.Model;
using IDecide.ViewModel;
using System;

namespace IDecide
{
    public static class AppContext
    {
        public static ObservableCollection<ChoiceGroupViewModel> Groups { get; set; }

        public static string GetRandomChoice()
        {
            if (!Groups.Any())
                return AppResources.NothingToDecide;

            var selectedChoices = Groups
                .Select(m => m.Model)
                .Where(c => c.IsSelected)
                .Single().Choices
                .ToList();

            if (!selectedChoices.Any())
                return AppResources.NothingToDecide;

            var rnd = new Random();
            return selectedChoices[rnd.Next(selectedChoices.Count)];
                
        }

        public static ObservableCollection<ChoiceGroupViewModel> GetDefaultChoices()
        {
            var Choices = new ObservableCollection<ChoiceGroupViewModel>();

            Choices.Add(new ChoiceGroupViewModel(new ChoiceGroup()
            {
                Name = "MagicBall",
                IsSelected = true,
                Choices = new ObservableCollection<string>(
                    Enumerable.Range(1, 20).Select(i =>
                    DefaultChoices.ResourceManager.GetString("MagicBall" + i)))
            }));

            Choices.Add(new ChoiceGroupViewModel(new ChoiceGroup()
            {
                Name = "Dice",
                Choices = new ObservableCollection<string>(
                    Enumerable.Range(1, 6).Select(i =>
                    DefaultChoices.ResourceManager.GetString("Dice" + i)))
            }));


            Choices.Add(new ChoiceGroupViewModel(new ChoiceGroup()
            {
                Name = "HeadTail",
                Choices = new ObservableCollection<string>(new[]
                {
                    DefaultChoices.Head,
                    DefaultChoices.Tail
                })
            }));

            Choices.Add(new ChoiceGroupViewModel(new ChoiceGroup()
            {
                Name = "YesNoMaybe",
                Choices = new ObservableCollection<string>(new[]
                { 
                    DefaultChoices.Yes,
                    DefaultChoices.No,
                    DefaultChoices.Maybe
                })
            }));

            Choices.Add(new ChoiceGroupViewModel(new ChoiceGroup()
            {
                Name = "RPSLS",
                Choices = new ObservableCollection<string>(new[]
                {
                    DefaultChoices.Rock, DefaultChoices.Paper, DefaultChoices.Scissor,
                    DefaultChoices.Lizard, DefaultChoices.Spock
                })
            }));

            Choices.Add(new ChoiceGroupViewModel(new ChoiceGroup()
            {
                Name = "Percentage",
                Choices = new ObservableCollection<string>(
                    Enumerable.Range(0, 100).Select(i => i + "%"))
            }));

            return Choices;
        }

    }
}