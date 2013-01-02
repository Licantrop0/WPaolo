using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TwentyTwelve_Organizer.Model;

namespace TwentyTwelve_Organizer.ViewModel
{
    public class AddEditTaskViewModel : ViewModelBase
    {
        public Task CurrentTask { get; set; }
        public bool IsEditMode { get { return CurrentTask.Description != null; } }

        public string Title { get { return IsEditMode ? "Edit Task" : "Add Task"; } }
        public string ActionText { get { return IsEditMode ? "Edit" : "Add"; } }
        public string Description { get { return CurrentTask.Description; } }

        public bool IsVerySimple
        {
            get { return CurrentTask.Difficulty == Model.TaskDifficulty.VerySimple; }
            set { if (value) CurrentTask.Difficulty = Model.TaskDifficulty.VerySimple; }
        }

        public bool IsSimple
        {
            get { return CurrentTask.Difficulty == Model.TaskDifficulty.Simple; }
            set { if (value) CurrentTask.Difficulty = Model.TaskDifficulty.Simple; }
        }

        public bool IsNormal
        {
            get { return CurrentTask.Difficulty == Model.TaskDifficulty.Normal; }
            set { if (value) CurrentTask.Difficulty = Model.TaskDifficulty.Normal; }
        }

        public bool IsHard
        {
            get { return CurrentTask.Difficulty == Model.TaskDifficulty.Hard; }
            set { if (value) CurrentTask.Difficulty = Model.TaskDifficulty.Hard; }
        }

        public bool IsVeryHard
        {
            get { return CurrentTask.Difficulty == Model.TaskDifficulty.VeryHard; }
            set { if (value) CurrentTask.Difficulty = Model.TaskDifficulty.VeryHard; }
        }

        private RelayCommand _addEditCommand;
        public RelayCommand AddEditCommand
        {
            get
            {
                return _addEditCommand ?? (_addEditCommand = new RelayCommand(() =>
                {
                    MessengerInstance.Send("", "GoBack");
                }));
            }
        }


    }
}
