using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.ComponentModel;
using System.Windows.Data;
using TwentyTwelve_Organizer.Model;
using WPCommon.Helpers;

namespace TwentyTwelve_Organizer.ViewModel
{
    public class TasksViewModel : ViewModelBase
    {
        public CollectionViewSource ToDoCVS { get; set; }
        public CollectionViewSource CompletedCVS { get; set; }
        SortDescription DescriptionAscending = new SortDescription("Description", ListSortDirection.Ascending);

        public TasksViewModel()
        {
            if (IsInDesignMode) return;

            ToDoCVS = new CollectionViewSource();
            ToDoCVS.Source = AppContext.Tasks;
            ToDoCVS.SortDescriptions.Add(DescriptionAscending);
            ToDoCVS.Filter += (sender, e) => { e.Accepted = !((TaskViewModel)e.Item).CurrentTask.IsCompleted; };

            CompletedCVS = new CollectionViewSource();
            CompletedCVS.Source = AppContext.Tasks;
            CompletedCVS.SortDescriptions.Add(DescriptionAscending);
            CompletedCVS.Filter += (sender, e) => { e.Accepted = ((TaskViewModel)e.Item).CurrentTask.IsCompleted; };

        }

        private RelayCommand _addCommand;
        public RelayCommand AddCommand
        {
            get
            {
                return _addCommand ?? (_addCommand = new RelayCommand(() =>
                {
                    if (TrialManagement.IsTrialMode)
                        MessengerInstance.Send(new Uri("/View/DemoInfoPage.xaml", UriKind.Relative), "navigate");
                    else
                        MessengerInstance.Send(new Uri("/View/AddEditTaskPage.xaml", UriKind.Relative), "navigate");
                }));
            }
        }

    }
}
