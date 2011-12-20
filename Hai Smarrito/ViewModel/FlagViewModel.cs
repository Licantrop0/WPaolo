using System.IO;
using System.Windows.Media.Imaging;
using HaiSmarrito.Helpers;

namespace HaiSmarrito.ViewModel
{
    public class FlagViewModel
    {
        public string Name { get; private set; }
        public BitmapImage FlagPic { get; private set; }
        public string Number { get; set; }

        public FlagViewModel(string rawName, string number, byte[] rawImage)
        {
            Name = rawName.Replace("_", " ");
            Number = number;
            FlagPic = new BitmapImage();
            FlagPic.SetSource(new MemoryStream(rawImage));
        }

        private RelayCommand<string> _call;
        public RelayCommand<string> Call
        {
            get
            {
                return _call ?? (_call = new RelayCommand<string>(CallAction));
            }
        }       

        private void CallAction(string cardType)
        {
            string callName = string.Format("{0} {1}",
                CreditCardHelper.GetName(cardType), Name);

            CallHelper.Call(callName, Number);
        }

    }
}