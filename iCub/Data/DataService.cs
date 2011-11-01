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
                    p.Attribute("Abstract").Value,
                    new Uri(p.Attribute("Url").Value)
                    ));
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
                    ));
        }

    }
}