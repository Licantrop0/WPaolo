using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media.Imaging;
using EasyCall.Model;

namespace EasyCall
{
    public class ContactViewModel : INotifyPropertyChanged, IGrouping<string, string>
    {
        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public Contact Model { get; private set; }
        public string SearchText { get; private set; }
        private int _indexName;

        public string Name1
        {
            get
            {
                if (_indexName == -1) return Model.DisplayName;
                return Model.DisplayName.Substring(0, _indexName);
            }
        }

        public string Name2
        {
            get
            {
                if (_indexName == -1) return string.Empty;
                return Model.DisplayName.Substring(_indexName, SearchText.Length); 
            }
        }

        public string Name3
        {
            get
            {
                if (_indexName == -1) return string.Empty;
                return Model.DisplayName.Substring(_indexName + SearchText.Length);
            }
        }
        
        public ContactViewModel(Contact contact, string searchText)
        {
            Model = contact;
            SearchText = searchText;
            _indexName = contact.FullNumberRepresentation.IndexOf(searchText);
        }

        #region IGrouping Implementation

        public string Key
        {
            get { return Model.DisplayName; }
        }

        public IEnumerator<string> GetEnumerator()
        {
            foreach (string n in Model.Numbers)
                yield return n;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Model.Numbers.GetEnumerator();
        }

        #endregion

        //Todo: quando il LongListSelector supporterà il binding sul SelectedItem...
        //public string SelectedNumber
        //{
        //    get { return null; }
        //    set
        //    {
        //        RaisePropertyChanged("SelectedNumber");
        //        CallHelper.Call(Model);
        //    }
        //}

    }
}