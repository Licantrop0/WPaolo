using System.Collections.ObjectModel;
using System.Windows;
using GalaSoft.MvvmLight;
using IDecide.Model;

namespace IDecide.ViewModel
{
    public class ChoiceGroupViewModel : ViewModelBase
    {
        //public ObservableCollection<string> Choices { get; set; }        
        public ChoiceGroup Model { get; private set; }
        public bool ButtonEnabled { get { return !Model.IsDefault; } }

        public Visibility CanDeleteVisibility
        {
            get
            {
                return Model.IsDefault ?
                    Visibility.Collapsed :
                    Visibility.Visible;
            }
        }

        public ChoiceGroupViewModel(ChoiceGroup model)
        {
            this.Model = model;
            //this.Choices = new ObservableCollection<string>();
            //foreach (var choice in Model.Choices)
            //    this.Choices.Add(choice);
        }
    }
}
