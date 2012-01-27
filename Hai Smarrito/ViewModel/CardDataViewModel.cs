using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Tasks;
using NientePanico.Helpers;
using NientePanico.Model;
using WPCommon.Helpers;

namespace NientePanico.ViewModel
{
    public class CardDataViewModel : INotifyPropertyChanged
    {
        public CardData CurrentCard { get; set; }
        public bool IsEditMode { get; set; }

        public CardDataViewModel()
        {
            CurrentCard = new CardData();
            IsEditMode = false;
        }

        public CardDataViewModel(CardData currentCard)
        {
            CurrentCard = currentCard;
            IsEditMode = true;
        }

        private RelayCommand _takePicture;
        public RelayCommand TakePicture
        {
            get { return _takePicture ?? (_takePicture = new RelayCommand(TakePictureAction)); }
        }

        private void TakePictureAction(object isFront)
        {
            var cameraCaptureTask = new CameraCaptureTask();
            cameraCaptureTask.Completed += (sender, e) =>
            {
                if (e.TaskResult != TaskResult.OK)
                    return;

                using (var pic = e.ChosenPhoto)
                {
                    SetPhoto(pic, bool.Parse(isFront.ToString()));
                }
            };
            try
            {
                cameraCaptureTask.Show();
            }
            catch (InvalidOperationException)
            { /*non posso farci niente */ };
        }

        public void SetPhoto(Stream stream, bool isFront)
        {
            var bitmap = new BitmapImage();
            bitmap.SetSource(stream);
            CurrentCard.CacheImage(stream, isFront);
            
            var ImageName = Guid.NewGuid().ToString();
            if (isFront) //this raise the PropertyChanged
                CurrentCard.FrontImageName = ImageName;
            else
                CurrentCard.BackImageName = ImageName;

            //Actually save the photo to the IsolatedStorage
            PhotoHelper.SavePhoto(ImageName, bitmap);
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
