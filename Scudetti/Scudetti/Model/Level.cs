using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace Scudetti.Model
{
    public class Level : IGrouping<int, Shield>, INotifyPropertyChanged
    {
        private int key;
        private IEnumerable<Shield> shields;

        public Level(IGrouping<int, Shield> group)
        {
            key = group.Key;
            shields = group;

            foreach (var shield in shields)
            {
                shield.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == "IsValidated")
                        RaisePropertyChanged("Completed");
                };
            }
        }

        public int Key
        {
            get { return key; }
        }

        private bool _isUnlocked;
        public bool IsUnlocked
        {
            get { return _isUnlocked; }
            set
            {
                if (IsUnlocked == value) return;
                _isUnlocked = value;
                RaisePropertyChanged("IsUnlocked");
            }
        }

        public int Count { get { return shields.Count(); } }
        public int Completed { get { return shields.Count(s => s.IsValidated); } }


        public System.Collections.Generic.IEnumerator<Shield> GetEnumerator()
        {
            return shields.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return shields.GetEnumerator();
        }

        #region Interface Implementations
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
