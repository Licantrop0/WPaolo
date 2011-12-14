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
            switch (cardType)
            {
                case "amex":
                    CallHelper.Call("American Express " + Name, Number);
                    break;
                case "visa":
                    CallHelper.Call("Visa " + Name, Number);
                    break;
                case "mastercard":
                    CallHelper.Call("Master Card " + Name, Number);
                    break;
                default:
                    break;
            }
        }

    }
}