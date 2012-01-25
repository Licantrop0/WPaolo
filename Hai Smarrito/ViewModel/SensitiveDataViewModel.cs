using System.ComponentModel;
using System.Collections.ObjectModel;
using WPCommon.Helpers;
using Microsoft.Phone.Tasks;
using System;

namespace NientePanico.ViewModel
{
    public class SensitiveDataViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<CardDataViewModel> Cards
        {
            get { return AppContext.Cards; }
        }

        private RelayCommand _takePicture;
        public RelayCommand TakePicture
        {
            get { return _takePicture ?? (_takePicture = new RelayCommand(TakePictureAction)); }
        }

        private void TakePictureAction(object args)
        {
            var cameraCaptureTask = new CameraCaptureTask();
            cameraCaptureTask.Completed += (sender1, e1) =>
            {
                if (e1.TaskResult == TaskResult.OK)
                {
                    var newCard = new CardDataViewModel();
                    using (var pic = e1.ChosenPhoto)
                    {
                        newCard.SetPhoto(pic);
                        Cards.Add(newCard);
                    }
                }
            };
            try
            {
                cameraCaptureTask.Show();
            }
            catch (InvalidOperationException) { };
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