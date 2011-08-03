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
using IDecide.Model;
using System.Collections.ObjectModel;

namespace IDecide.ViewModel
{
    public class ChoiceGroupViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<string> Choices { get; set; }
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
            this.Choices = new ObservableCollection<string>();
            foreach (var choice in Model.Choices)
                this.Choices.Add(choice);
        }
    }
}
