using System;
using EasyCall.Model;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace EasyCall
{
    public static class AppContext
    {
        public static IList<Contact> Contacts;
        public static IList<ContactViewModel> GetFakeContacts()
        {
            return new List<ContactViewModel>()
            {
                new ContactViewModel(new Model.Contact("Luca Spolidoro", new[] {"393 3714189", "010 311540"}, null), ""),
                new ContactViewModel(new Model.Contact("Pino Quercia", new[] {"+39 384 30572194", "+39 02 365688", "02 074 234456"}, null), ""),
                new ContactViewModel(new Model.Contact("Sapporo Piloro", new[] {"347 5840382"}, null), "")
            };
        }

        public static List<ContactViewModel> Filter(string searchedText)
        {
            if (AppContext.Contacts == null || string.IsNullOrEmpty(searchedText))
            {
                return new List<ContactViewModel>();
            }

            return (from contact in Contacts
                    where contact.ContainsName(searchedText) ||
                          contact.ContainsNumber(searchedText)
                    select new ContactViewModel(contact, searchedText)
                    ).ToList();
        }

    }
}
