using System;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;
using iCub.Helpers;

namespace iCub.ViewModel
{
    public class ContactViewModel : ViewModelBase
    {
        public string Name { get; private set; }
        public Uri Image { get; private set; }
        public string Mail { get; private set; }
        public string Description { get; private set; }

        public ContactViewModel(string name, Uri image, string mail, string description)
        {
            Name = name;
            Image = image;
            Mail = mail;
            Description = description;
        }

        private RelayCommand _sendMail;
        public RelayCommand SendMail
        {
            get { return _sendMail ?? (_sendMail = new RelayCommand(() => LinksHelper.ComposeMail(Mail))); }
        }
    }
}
