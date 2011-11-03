using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using iCub.Data;

namespace iCub.ViewModel
{
    public class MainPanoramaViewModel : ViewModelBase
    {
        public IEnumerable<PaperViewModel> Papers { get; private set; }
        public ObservableCollection<ProjectViewModel> Projects { get; private set; }
        public IEnumerable<ContactViewModel> Contacts { get; private set; }

        public MainPanoramaViewModel()
        {
            Papers = DataService.GetPapers();
            Projects = new ObservableCollection<ProjectViewModel>(DataService.GetProjects());
            Contacts = DataService.GetContacts();
        }

        public ProjectViewModel SelectedProject
        {
            get { return null; }

            set
            {
                foreach (var project in Projects.Except(new[] { value }))
                {
                    project.IsExpanded = false;
                }
                RaisePropertyChanged("SelectedProject");
            }
        }
    }
}