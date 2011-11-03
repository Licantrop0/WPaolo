using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using iCub.Helpers;

namespace iCub.ViewModel
{
    public class PaperViewModel : ViewModelBase
    {
        public string Title { get; private set; }
        public string Authors { get; private set; }
        public string Abstr { get; private set; }
        public string Location { get; private set; }
        public int Year { get; private set; }
        public Uri Url { get; private set; }

        public PaperViewModel(string title, string authors, string abstr, string location, int year, Uri url)
        {
            Title = title;
            Authors = authors;
            Abstr = abstr;
            Location = location;
            Year = year;
            Url = url;
        }

        private RelayCommand _openUrl;
        public RelayCommand OpenUrl
        {
            get { return _openUrl ?? (_openUrl = new RelayCommand(() => LinksHelper.OpenUrl(Url))); }
        }
    }
}