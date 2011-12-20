using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using HaiSmarrito.Images.Flags;

namespace HaiSmarrito.ViewModel
{
    public class NazioniViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<FlagViewModel> Flags { get; set; }

        private string _cardType;
        public string CardType
        {
            get { return _cardType; }
            set
            {
                _cardType = value;
                LoadFlags();
            }
        }

        private int CardIndex
        {
            get
            {
                if (CardType == "amex") return 1;
                else if (CardType == "visa") return 2;
                else return 3; //CardType == "mastercard"
            }
        }

        private void LoadFlags()
        {
            Flags = new ObservableCollection<FlagViewModel>(
                from de in FlagsResource.ResourceManager
                    .GetResourceSet(CultureInfo.CurrentCulture, true, true)
                    .Cast<DictionaryEntry>()
                let values = de.Key.ToString().Split('|')
                where !string.IsNullOrEmpty(values[CardIndex])
                orderby values[0]
                select new FlagViewModel(values[0], values[CardIndex], (byte[])de.Value));
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