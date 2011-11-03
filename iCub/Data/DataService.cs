using System;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using iCub.ViewModel;

namespace iCub.Data
{
    public static class DataService
    {
        public static IEnumerable<PaperViewModel> GetPapers()
        {
            XDocument XPapers = XDocument.Load("Data/Papers.xml");

            return XPapers.Descendants("Paper").Select(p =>
                new PaperViewModel(
                    p.Attribute("Title").Value,
                    p.Attribute("Authors").Value,
                    p.Attribute("Abstract").Value,
                    p.Attribute("Location").Value,
                    int.Parse(p.Attribute("Year").Value),
                    new Uri(p.Attribute("Url").Value)
                    )).OrderByDescending(p => p.Year);
        }

        public static IEnumerable<ProjectViewModel> GetProjects()
        {
            XDocument XProjects = XDocument.Load("Data/Projects.xml");

            return XProjects.Descendants("Project").Select(p =>
                new ProjectViewModel(
                    p.Attribute("Name").Value,
                    p.Attribute("Description").Value,
                    new Uri(p.Attribute("Logo").Value, UriKind.Relative),
                    new Uri(p.Attribute("Url").Value)
                    )).OrderBy(p => p.Name);
        }

        public static IEnumerable<ContactViewModel> GetContacts()
        {
            XDocument XContacts = XDocument.Load("Data/Contacts.xml");

            return XContacts.Descendants("Contact").Select(p =>
                new ContactViewModel(
                    p.Attribute("Name").Value,
                    new Uri(p.Attribute("Image").Value, UriKind.Relative),
                    p.Attribute("Mail").Value
                    ));
        }

    }
}