﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using iCub.Helpers;

namespace iCub.ViewModel
{
    public class ProjectViewModel : ViewModelBase
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public Uri Logo { get; private set; }
        public Uri Url { get; private set; }

        public ProjectViewModel(string name, string descr, Uri logo, Uri url)
        {
            Name = name;
            Description = descr;
            Logo = logo;
            Url = url;
        }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }

            set
            {
                if (_isExpanded == value)
                    return;

                _isExpanded = value;
                RaisePropertyChanged("IsExpanded");
            }
        }

        private RelayCommand _openUrl;
        public RelayCommand OpenUrl
        {
            get { return _openUrl ?? (_openUrl = new RelayCommand(() => LinksHelper.OpenUrl(Url))); }
        }
    }
}
