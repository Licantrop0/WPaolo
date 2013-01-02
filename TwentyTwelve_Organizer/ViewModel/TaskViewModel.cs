using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using TwentyTwelve_Organizer.Model;
using WPCommon.Helpers;

namespace TwentyTwelve_Organizer.ViewModel
{
    public class TaskViewModel : ViewModelBase
    {
        public int Id { get { return CurrentTask.Description.GetHashCode() ^ CurrentTask.IsCompleted.GetHashCode(); } }

        public Task CurrentTask { get; set; }

        public TaskViewModel(Task task)
        {
            CurrentTask = task;
        }

        private RelayCommand _removeCommand;
        public RelayCommand RemoveCommand
        {
            get
            {
                return _removeCommand ?? (_removeCommand = new RelayCommand(() =>
                {
                    if (TrialManagement.IsTrialMode)
                        MessengerInstance.Send(new Uri("/View/DemoInfoPage.xaml", UriKind.Relative), "navigate");
                    else
                        if (MessageBox.Show("Do you want to delete this taks?", "Confirm", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            AppContext.Tasks.Remove(this);
                        }

                }));
            }
        }

        private RelayCommand _goToTaskCommand;
        public RelayCommand GoToTaskCommand
        {
            get
            {
                return _goToTaskCommand ?? (_goToTaskCommand = new RelayCommand(() =>
                {
                    if (TrialManagement.IsTrialMode)
                        MessengerInstance.Send(new Uri("/View/DemoInfoPage.xaml", UriKind.Relative), "navigate");
                    else
                    {
                        MessengerInstance.Send(new Uri("/View/AddEditTaskPage.xaml?id=" + Id, UriKind.Relative), "navigate");
                    }
                }));
            }
        }
    }
}
