using System.Collections.ObjectModel;
using System.Linq;
using IDecide.Localization;
using IDecide.Model;
using IDecide.ViewModel;

namespace IDecide
{
    public class AppContext
    {
        private static ObservableCollection<ChoiceGroupViewModel> _groups;
        public static ObservableCollection<ChoiceGroupViewModel> Groups
        {
            get
            {
                if (_groups == null)
                    _groups = GetDefaultChoices();
                return _groups;
            }
            set { _groups = value; }
        }



        public static ObservableCollection<ChoiceGroupViewModel> GetDefaultChoices()
        {
            var Choices = new ObservableCollection<ChoiceGroupViewModel>();

            Choices.Add(new ChoiceGroupViewModel(new ChoiceGroup()
            {
                Name = "MagicBall",
                IsDefault = true,
                IsSelected = true,
                Choices = Enumerable.Range(1, 20).Select(i =>
                    DefaultChoices.ResourceManager.GetString("MagicBall" + i))
            }));

            Choices.Add(new ChoiceGroupViewModel(new ChoiceGroup()
            {
                Name = "HeadTail",
                IsDefault = true,
                Choices = new string[] { DefaultChoices.Head, DefaultChoices.Tail }
            }));

            Choices.Add(new ChoiceGroupViewModel(new ChoiceGroup()
            {
                Name = "YesNoMaybe",
                IsDefault = true,
                Choices = new string[]
                { 
                    DefaultChoices.Yes,
                    DefaultChoices.No,
                    DefaultChoices.Maybe
                }
            }));

            Choices.Add(new ChoiceGroupViewModel(new ChoiceGroup()
            {
                Name = "RPSLS",
                IsDefault = true,
                Choices = new string[]
                {
                    DefaultChoices.Rock, DefaultChoices.Paper, DefaultChoices.Scissor,
                    DefaultChoices.Lizard, DefaultChoices.Spock
                }
            }));

            Choices.Add(new ChoiceGroupViewModel(new ChoiceGroup()
            {
                Name = "Percentage",
                IsDefault = true,
                Choices = Enumerable.Range(0, 100).Select(i => i + "%")
            }));

            return Choices;
        }
    }
}
