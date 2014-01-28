using GalaSoft.MvvmLight;
using Microsoft.Phone.BackgroundTransfer;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using WPCommon.Helpers;

namespace SgarbiMix.WP7.ViewModel
{
    public class UpdateViewModel : ViewModelBase
    {
        INavigationService _navigationService;

        public ObservableCollection<TransferMonitor> Downloads { get; set; }
        private int AllFilesCount;
        const string baseUri = "shared/transfers/";

        public string Title
        {
            get
            {
                if (AllFilesCount == 0)
                    return "Carico la lista Insulti...";
                return string.Format("Aggiorno Insulti {0}/{1}",
                    Downloads.Count, AllFilesCount);
            }
        }

        Queue<BackgroundTransferRequest> TransferQueue;

        public UpdateViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            if (DesignerProperties.IsInDesignTool)
            {
                var btr = new BackgroundTransferRequest(
                    new Uri("http://206.72.115.176/SgarbiMix/sounds.xml"),
                    new Uri(baseUri + "sounds.xml", UriKind.Relative));
                Downloads = new ObservableCollection<TransferMonitor>()
                {
                    new TransferMonitor(btr),
                    new TransferMonitor(btr),
                    new TransferMonitor(btr)
                };
                return;
            }
            if (!DeviceNetworkInformation.IsNetworkAvailable)
            {
                MessageBox.Show("Mi spiace ma devi essere connesso a internet per scaricare i nuovi insulti!");
                return;
            }

            Downloads = new ObservableCollection<TransferMonitor>();
            Downloads.CollectionChanged += (sender, e) => RaisePropertyChanged("Title");
            GetFileList();
        }

        private async void GetFileList()
        {
            //remove previous backgroundtransfers pending
            foreach (var request in BackgroundTransferService.Requests)
            {
                BackgroundTransferService.Remove(request);
            }

            SoundViewModel[] sounds;
            using (var newXml = await AppContext.GetNewXmlAsync())
                sounds = AppContext.SoundSerializer.Deserialize(newXml) as SoundViewModel[];

            var differences = sounds.Select(s => s.File)
                .Except(GetNonEmptyFiles())
                .Concat(new[] { "Sounds.xml" });

            AllFilesCount = differences.Count();
            if (AllFilesCount == 0)
            {
                MessageBox.Show("Gli insulti sono già tutti aggiornati!");
                _navigationService.GoBack();
                return;
            }

            TransferQueue = new Queue<BackgroundTransferRequest>(
                differences.Select(file => new BackgroundTransferRequest(
                    new Uri("http://206.72.115.176/SgarbiMix/" + file),
                    new Uri(baseUri + file, UriKind.Relative))
                    {
                        TransferPreferences = TransferPreferences.AllowCellularAndBattery
                    }));

            for (int i = 0; i < Math.Min(5, TransferQueue.Count); i++)
                StartDownload(TransferQueue.Dequeue());
        }

        private List<string> GetNonEmptyFiles()
        {
            var fileList = new List<string>();
            using (var isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                var files = isf.GetFileNames(baseUri + "*.wav");
                foreach (var file in files)
                {
                    //using (var f = isf.OpenFile(file, FileMode.Open))
                    //if (f.Length > 0)
                    fileList.Add(file);
                }
            }
            return fileList;
        }

        private void StartDownload(BackgroundTransferRequest btr)
        {
            var name = Path.GetFileNameWithoutExtension(
                btr.DownloadLocation.OriginalString).Replace("_", " ");
            var tm = new TransferMonitor(btr, name);
            tm.Complete += tm_Complete;
            Downloads.Insert(0, tm);
            tm.RequestStart();
        }

        void tm_Complete(object sender, BackgroundTransferEventArgs e)
        {
            try
            {
                BackgroundTransferService.Remove(e.Request);
            }
            catch (ObjectDisposedException) //se qualcosa va storto boh...
            { }

            if (TransferQueue.Count != 0)
                StartDownload(TransferQueue.Dequeue());

            if (!BackgroundTransferService.Requests.Any())
            {
                MessengerInstance.Send("update_completed");
                MessageBox.Show("Ora puoi insultare con nuovi insulti!", "Download Completato", MessageBoxButton.OK);
                _navigationService.GoBack();
            }
        }
    }
}
