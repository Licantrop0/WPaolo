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
        private static readonly string letters = "#abcdefghijklmnopqrstuvwxyz";

        public string CardType { get; set; }
        public string CreditCardName
        {
            get { return CreditCardHelper.GetName(CardType); }
        }

        private IEnumerable<LLSGroup<FlagViewModel>> _flags;
        public IEnumerable<LLSGroup<FlagViewModel>> Flags
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

        private IEnumerable<LLSGroup<FlagViewModel>> LoadFlags()
        {
            var CardIndex = CreditCardHelper.GetIndex(CardType);

            return (from de in FlagsResource.ResourceManager
                        .GetResourceSet(CultureInfo.CurrentCulture, true, true)
                        .Cast<DictionaryEntry>()
                    let values = de.Key.ToString().Split('|')
                    where !string.IsNullOrEmpty(values[CardIndex])
                    let flag = new FlagViewModel(values[0], values[CardIndex], (byte[])de.Value)
                    orderby flag.Name //ordino le bandierine per nome
                    group flag by char.ToLower(flag.Name[0]) into g //le raggruppo per iniziale
                    select new LLSGroup<FlagViewModel>(g))
                    .Union(letters.Select(l => new LLSGroup<FlagViewModel>(l))) //ci aggiungo le lettere mancanti
                    .OrderBy(g => g.Key); //ordino per iniziale del gruppo
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