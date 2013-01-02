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
        public bool IsEditMode { get { return CurrentTask != null; } }

        public string Title { get { return IsEditMode ? "Edit Task" : "Add Task"; } }
        public string ActionText { get { return IsEditMode ? "Edit" : "Add"; } }

        public AddEditTaskViewModel()
        {
            MessengerInstance.Register<Task>(this, t => CurrentTask = t);
        }

        #region CheckBoxes

        public bool IsVerySimple
        {
            get
            {
                return IsEditMode ?
                    CurrentTask.Difficulty == Model.TaskDifficulty.VerySimple : false;
            }
            set { if (value) CurrentTask.Difficulty = Model.TaskDifficulty.VerySimple; }
        }

        public bool IsSimple
        {
            get
            {
                return IsEditMode ?
                    CurrentTask.Difficulty == Model.TaskDifficulty.Simple : false;
            }
            set { if (value) CurrentTask.Difficulty = Model.TaskDifficulty.Simple; }
        }

        public bool IsNormal
        {
            get
            {
                return IsEditMode ?
                    CurrentTask.Difficulty == Model.TaskDifficulty.Normal : true;
            }
            set { if (value) CurrentTask.Difficulty = Model.TaskDifficulty.Normal; }
        }

        public bool IsHard
        {
            get
            {
                return IsEditMode ?
                    CurrentTask.Difficulty == Model.TaskDifficulty.Hard : false;
            }
            set { if (value) CurrentTask.Difficulty = Model.TaskDifficulty.Hard; }
        }

        public bool IsVeryHard
        {
            get
            {
                return IsEditMode ?
                    CurrentTask.Difficulty == Model.TaskDifficulty.VeryHard : false;
            }
            set { if (value) CurrentTask.Difficulty = Model.TaskDifficulty.VeryHard; }
        }

        #endregion

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
