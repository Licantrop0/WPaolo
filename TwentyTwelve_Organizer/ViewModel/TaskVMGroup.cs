using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TwentyTwelve_Organizer.Model;

namespace TwentyTwelve_Organizer.ViewModel
{
    public class TaskVMGroup : IGrouping<string, TaskViewModel>, IEnumerable<TaskViewModel>
    {
        public string Key { get; set; }
        public bool HasElements { get { return _elements.Any(); } }

        private readonly IEnumerable<TaskViewModel> _elements;

        public TaskVMGroup(IGrouping<bool, Task> internalGrouping)
        {
            Key = internalGrouping.Key ? "Completed" : "To Do";
            _elements = internalGrouping.Select(t => new TaskViewModel(t));
        }

        public override bool Equals(object obj)
        {
            var that = obj as TaskVMGroup;
            return (that != null) && (this.Key.Equals(that.Key));
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        #region IEnumerable Members
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        IEnumerator<TaskViewModel> IEnumerable<TaskViewModel>.GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        #endregion
    }
}
