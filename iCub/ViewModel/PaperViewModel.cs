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
        public string Abstr { get; private set; }
        public Uri Url { get; private set; }

        public PaperViewModel(string title, string abstr, Uri url)
        {
            Title = title;
            Abstr = abstr;
            Url = url;
        }

        private RelayCommand _openUrl;
        public RelayCommand OpenUrl
        {
            get { return _openUrl ?? (_openUrl = new RelayCommand(() => WebBrowserHelper.Open(Url))); }
        }
    }
}
