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
       
        private void LoadFlags()
        {
            var resources = from de in FlagsResource.ResourceManager
                                .GetResourceSet(CultureInfo.CurrentCulture, true, true)
                                .Cast<DictionaryEntry>()
                            orderby de.Key
                            select new { values = de.Key.ToString().Split('|'), bytes = (byte[])de.Value };

            Flags = new ObservableCollection<FlagViewModel>();
            foreach (var res in resources)
            {
                string number = string.Empty;
                switch (CardType)
                {
                    case "amex":
                        if (string.IsNullOrEmpty(res.values[1]))
                            continue;
                        else
                            number = res.values[1];
                        break;

                    case "visa":
                        if (string.IsNullOrEmpty(res.values[2]))
                            continue;
                        else
                            number = res.values[2];
                        break;

                    case "mastercard":
                        if (string.IsNullOrEmpty(res.values[3])) 
                            continue;
                        else
                            number = res.values[3];
                        break;

                    default:
                        break;
                }

                Flags.Add(new FlagViewModel(res.values[0], number, res.bytes));
            }

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
