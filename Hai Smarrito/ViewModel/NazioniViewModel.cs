using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using HaiSmarrito.Images.Flags;
using System.Collections.ObjectModel;

namespace HaiSmarrito.ViewModel
{
    public class NazioniViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<FlagViewModel> Flags { get; set; }
        public string CardType { get; set; }

        public NazioniViewModel()
        {
            LoadFlags();
        }

        private void LoadFlags()
        {
             Flags = new ObservableCollection<FlagViewModel>(
                 from de in FlagsResource.ResourceManager
                     .GetResourceSet(CultureInfo.CurrentCulture, true, true)
                     .Cast<DictionaryEntry>()
                 orderby de.Key
                 select new FlagViewModel(de.Key.ToString(), (byte[])de.Value));
        }

        #region INPC Implementation

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        
        #endregion
    }
}
