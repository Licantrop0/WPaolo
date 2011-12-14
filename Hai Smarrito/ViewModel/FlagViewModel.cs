using System.IO;
using System.Windows.Media.Imaging;
using HaiSmarrito.Helpers;

namespace HaiSmarrito.ViewModel
{
    public class FlagViewModel
    {
        public string Name { get; private set; }
        public BitmapImage FlagPic { get; private set; }

        public string Type { get; set; }
        public string AmexNum { get; set; }
        public string VisaNum { get; set; }
        public string MasterCardNum { get; set; }

        public FlagViewModel(string rawName, byte[] rawImage)
        {
            var values = rawName.Split('|');
            Name = values[0].Replace("_", " ");

            if (values.Length > 2)
            {
                AmexNum = values[1];
                VisaNum = values[2];
                MasterCardNum = values[3];
            }
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
                    CallHelper.Call("American Express " + Name, AmexNum);
                    break;
                case "visa":
                    CallHelper.Call("Visa " + Name, VisaNum);
                    break;
                case "mastercard":
                    CallHelper.Call("MasterCard " + Name, MasterCardNum);
                    break;
                default:
                    break;
            }
        }

    }
}