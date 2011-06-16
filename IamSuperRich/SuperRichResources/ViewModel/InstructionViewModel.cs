using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Linq;
using SuperRichResources.Model;

namespace SuperRichResources.ViewModel
{
    public class InstructionViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string[] appId =
        {
            "59073daf-bb2b-e011-854c-00237de2db9e",
            "a9fb8160-c22b-e011-854c-00237de2db9e",
            "4941be10-c32b-e011-854c-00237de2db9e",
            "29c30692-c42b-e011-854c-00237de2db9e",
            "9996581f-c52b-e011-854c-00237de2db9e",
            "d982f2a3-c52b-e011-854c-00237de2db9e",
            "a921e145-c62b-e011-854c-00237de2db9e",
            "e9376fb6-c62b-e011-854c-00237de2db9e"
        };

         public IEnumerable<AppTile> ApplicationList
        {
            get
            {
                return from i in Enumerable.Range(1, 8)
                       select new AppTile
                       {
                           Img = new Uri("/SuperRichResources;component/numbered/" + i + ".png", UriKind.Relative),
                           AppId = appId[i - 1]
                       };
            }
        }

       public string PersonalText //Salvata sull'isolatedStorage
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("personal_text"))
                {
                    IsolatedStorageSettings.ApplicationSettings.Add(
                        "personal_text", "I am so rich that I can buy a useless app just because I can!");
                }
                return IsolatedStorageSettings.ApplicationSettings["personal_text"].ToString();
            }
            set
            {
                if (PersonalText == value)
                    return;

                IsolatedStorageSettings.ApplicationSettings["personal_text"] = value;
                RaisePropertyChanged("PersonalText");
            }
        }


    }
}
