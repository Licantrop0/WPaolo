using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using NientePanico.Helpers;
using NientePanico.Images.Flags;

namespace NientePanico.ViewModel
{
    public class NazioniViewModel : INotifyPropertyChanged
    {
        public string CardType { get; set; }
        public string CreditCardName
        {
            get { return CreditCardHelper.GetName(CardType); }
        }

        private ILookup<char, FlagViewModel> _flags;
        public ILookup<char, FlagViewModel> Flags
        {
            get
            {
                if (_flags == null)
                    _flags = LoadFlags();
                
                return _flags;
            }
            set
            {
                if (_flags == value) return;
                _flags = value;
                RaisePropertyChanged("Flags");
            }
        }

        public NazioniViewModel()
        {
            if (DesignerProperties.IsInDesignTool)
                CardType = "amex";
        }

        private ILookup<char, FlagViewModel> LoadFlags()
        {
            var CardIndex = CreditCardHelper.GetIndex(CardType);

            return (from de in FlagsResource.ResourceManager
                       .GetResourceSet(CultureInfo.CurrentCulture, true, true)
                       .Cast<DictionaryEntry>()
                    let values = de.Key.ToString().Split('|')
                    where !string.IsNullOrEmpty(values[CardIndex])
                    orderby values[0]
                    select new FlagViewModel(values[0], values[CardIndex], (byte[])de.Value))
                   .ToLookup(k => char.ToLower(k.Name[0]), v => v);
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