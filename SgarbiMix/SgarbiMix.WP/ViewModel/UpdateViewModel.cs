﻿using GalaSoft.MvvmLight;
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

namespace SgarbiMix.WP.ViewModel
{
    public class UpdateViewModel : ViewModelBase
    {
        readonly INavigationService _navigationService;
        const string baseUri = "shared/transfers/";

        public ObservableCollection<TransferMonitor> Downloads { get; set; }
        private int _allFilesCount;

        public string Title
        {
            get
            {
                return _allFilesCount == 0 ?
                    "Carico la lista Insulti..." :
                    $"Aggiorno Insulti {Downloads.Count}/{_allFilesCount}";
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
                MessageBox.Show("Mi spiace ma devi essere connesso a internet per scaricare i nuovi insulti.\nEsci, connettiti, e riapri l'app.");
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
            {
                if (newXml == null)
                {
                    MessageBox.Show("Whoops! c'è qualcosa che non va con la connessione al server degli insulti...\nRiprova fra un po'!\nSe ancora non funge, contattami su Facebook (WP Mobile Entertainment)");
                    AppContext.CloseApp();
                }
                sounds = (SoundViewModel[])AppContext.SoundSerializer.Deserialize(newXml);
            }

            var differences = sounds.Select(s => s.File)
                .Except(GetNonEmptyFiles())
                .Concat(new[] { "Sounds.xml" });

            _allFilesCount = differences.Count();
            if (_allFilesCount == 0)
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

            for (var i = 0; i < Math.Min(5, TransferQueue.Count); i++)
            StartDownload(TransferQueue.Dequeue());
        }

        private static IEnumerable<string> GetNonEmptyFiles()
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

        bool _isFinished = false;

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

            if (_isFinished) return;
            if (BackgroundTransferService.Requests.Any()) return;

            _isFinished = true;
            MessengerInstance.Send("update_completed");
            MessageBox.Show("Evvai!, ora puoi insultare con nuovi insulti!", "Download Completato", MessageBoxButton.OK);
            _navigationService.GoBack();
        }
    }
}
