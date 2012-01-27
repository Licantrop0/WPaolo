using System.ComponentModel;
using System.Collections.ObjectModel;
using WPCommon.Helpers;
using Microsoft.Phone.Tasks;
using System;
using NientePanico.Model;

namespace NientePanico.ViewModel
{
    public class SensitiveDataViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<CardDataViewModel> Cards
        {
            get
            {
                if (DesignerProperties.IsInDesignTool)
                    return new ObservableCollection<CardDataViewModel>(new[]
                    {
                        new CardDataViewModel(new CardData()
                        { Name= "Visa Bancomat", Pin="3432", Code="1231231312312", Expire = new DateTime(2015, 3, 23) }),
                        new CardDataViewModel(new CardData()
                        { Name= "American Express", Code="4356656654", Expire = new DateTime(2015, 3, 23) }),
                        new CardDataViewModel(new CardData()
                        { Name= "Patente", Code="66333", Expire = new DateTime(2013, 2, 2) })
                    });
                else
                    return AppContext.Cards;
            }
        }

        private RelayCommand _addDocument;
        public RelayCommand AddDocument
        {
            get { return _addDocument ?? (_addDocument = new RelayCommand(AddDocumentAction)); }
        }

        private void AddDocumentAction(object args)
        {

        }



        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}