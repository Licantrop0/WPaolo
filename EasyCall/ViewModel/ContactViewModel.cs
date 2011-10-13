using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media.Imaging;
using Microsoft.Phone;

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

        public Model.Contact Model { get; private set; }
        public string SearchText { get; private set; }

        private WriteableBitmap _bitmap;
        public WriteableBitmap Bitmap
        {
            get
            {
                if (_bitmap == null && Model.ImageStream != null)
                    _bitmap = PictureDecoder.DecodeJpeg(Model.ImageStream);

                return _bitmap;
            }
        }

        public ContactViewModel(Model.Contact contact, string searchText)
        {
            Model = contact;
            SearchText = searchText;
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
        //        CallHelper.Call(DisplayName, value);
        //    }
        //}

    }
}