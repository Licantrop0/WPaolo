﻿using Microsoft.Phone.BackgroundTransfer;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using SheldonMix.Localization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using WPCommon.Helpers;

namespace SheldonMix.ViewModel
{
    public class UpdateViewModel : ObservableObject
    {
        public ObservableCollection<TransferMonitor> Downloads { get; set; }
        private Queue<BackgroundTransferRequest> TransferQueue;
        private int AllFilesCount;
        const string baseUri = "shared/transfers/";

        public string Title
        {
            get
            {
                if (AllFilesCount == 0)
                    return AppResources.LoadingSoundsList;
                return string.Format(AppResources.UpdatingSounds,
                    Downloads.Count, AllFilesCount);
            }
        }

        public UpdateViewModel()
        {
            if (DesignerProperties.IsInDesignTool)
            {
                var btr = new BackgroundTransferRequest(
                    new Uri("http://206.72.115.176/SheldonMix/Sounds.xml"),
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
                MessageBox.Show(AppResources.NoConnectionAvailable);
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
                    MessageBox.Show(AppResources.ServerError);
                    Application.Current.Terminate();
                }
                sounds = AppContext.SoundSerializer.Deserialize(newXml) as SoundViewModel[];
            }
            var differences = sounds
                .Where(s => s.Lang == AppContext.Lang)
                .Select(s => s.File)
                .Except(GetNonEmptyFiles())
                .Concat(new[] { "Sounds.xml" });

            AllFilesCount = differences.Count();
            if (AllFilesCount == 0)
            {
                MessageBox.Show(AppResources.AlreadyUpdated);
                Ns.GoBack();
                return;
            }

            TransferQueue = new Queue<BackgroundTransferRequest>(
                differences.Select(file => new BackgroundTransferRequest(
                    new Uri("http://206.72.115.176/SheldonMix/" + file),
                    new Uri(baseUri + file, UriKind.Relative))
                {
                    TransferPreferences = TransferPreferences.AllowCellularAndBattery
                }));

            for (var i = 0; i < Math.Min(5, TransferQueue.Count); i++)
                StartDownload(TransferQueue.Dequeue());
        }

        private List<string> GetNonEmptyFiles()
        {
            var fileList = new List<string>();
            using (var isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                var files = isf.GetFileNames(baseUri + "*.mp3");
                //using (var f = isf.OpenFile(file, FileMode.Open))
                //if (f.Length > 0)
                fileList.AddRange(files);
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

        private bool _isFinished = false;

        private void tm_Complete(object sender, BackgroundTransferEventArgs e)
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
            AppContext.UpdateUI(this);
            MessageBox.Show(AppResources.NewSoundsDownloaded,
                AppResources.DownloadCompleted, MessageBoxButton.OK);
            Ns.GoBack();
        }
    }
}
