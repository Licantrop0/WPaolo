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
        public string DinersClubNum { get; set; }

        public FlagViewModel(string name, byte[] rawImage)
        {
            Name = name.Replace("_", " ");
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
                case "dinersclub":
                    CallHelper.Call("DinersClub " + Name, DinersClubNum);
                    break;

                default:
                    break;
            }
        }

    }
}