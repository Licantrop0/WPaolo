using EasyCall.Helper;
using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Windows.Input;
using WPCommon.Helpers;

namespace EasyCall.ViewModel
{
    [DataContract]
    public class NumberViewModel : IEquatable<NumberViewModel>
    {
        [DataMember]
        public string Number { get; set; }
        [DataMember]
        public string Name { get; set; }

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

        public bool Equals(NumberViewModel other)
        {
            return this.Name == other.Name && this.Number == other.Number;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode() ^ this.Number.GetHashCode();
        }
    }
}