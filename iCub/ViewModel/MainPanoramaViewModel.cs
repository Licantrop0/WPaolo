using GalaSoft.MvvmLight;
using iCub.Data;
using System.Collections.Generic;
using System;
using System.Linq;

namespace iCub.ViewModel
{
    public class MainPanoramaViewModel : ViewModelBase
    {
        public IEnumerable<PaperViewModel> Papers { get; private set; }
        public IEnumerable<ProjectViewModel> Projects { get; private set; }

        public MainPanoramaViewModel()
        {
            Papers = DataService.GetPapers();
            Projects = DataService.GetProjects();
        }
    }
}