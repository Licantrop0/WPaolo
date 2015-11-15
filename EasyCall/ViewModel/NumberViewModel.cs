using System.Text.RegularExpressions;
using System.Windows.Input;
using EasyCall.Helper;
using WPCommon.Helpers;

namespace EasyCall.ViewModel
{
    public class NumberViewModel
    {
        public string Number { get; }
        public string Name { get; }

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