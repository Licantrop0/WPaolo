using EasyCall.Helper;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Windows.Input;
using WPCommon.Helpers;

namespace EasyCall.ViewModel
{
    [DataContract]
    public class NumberViewModel
    {
        [DataMember]
        public string Number { get; set; }
        public string Name { get; }

        public NumberViewModel()
        {
            //for serialization
        }

        public NumberViewModel(string number, string name)
        {
            Number = Regex.Replace(number, @"[\s\-\(\)]", string.Empty);
            Name = name;
        }

        private ICommand _callNumberCommand;
        public ICommand CallNumberCommand => _callNumberCommand ??
            (_callNumberCommand = new RelayCommand(
                o => CallHelper.Call(Name, Number)));

        private ICommand _sendSmsCommand;
        public ICommand SendSmsCommand => _sendSmsCommand ??
            (_sendSmsCommand = new RelayCommand(
                o => CallHelper.SendSms(Number)));
    }
}