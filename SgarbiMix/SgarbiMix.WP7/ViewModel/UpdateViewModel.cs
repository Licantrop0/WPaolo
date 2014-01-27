using Microsoft.Phone.BackgroundTransfer;
using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Windows;

namespace SgarbiMix.WP7.ViewModel
{
    public class UpdateViewModel
    {
        public ObservableCollection<TransferMonitor> Downloads { get; set; }
        const string baseUri = "shared/transfers/";
        Queue<BackgroundTransferRequest> TransferQueue;
        public UpdateViewModel()
        {
            if (DesignerProperties.IsInDesignTool) return;

            Downloads = new ObservableCollection<TransferMonitor>();
            GetFileList();
        }

        private async void GetFileList()
        {
            var newXml = await AppContext.GetNewXmlAsync();
            var sounds = AppContext.SoundSerializer.Deserialize(newXml) as SoundViewModel[];
            newXml.Close();

            var isf = IsolatedStorageFile.GetUserStoreForApplication();
            var existings = isf.GetFileNames(baseUri + "*.wav");
            var differences = sounds.Select(s => s.File)
                .Except(existings.Select(f => HttpUtility.UrlDecode(f)))
                .Concat(new[] { "Sounds.xml" });

            TransferQueue = new Queue<BackgroundTransferRequest>(
                differences.Select(file => new BackgroundTransferRequest(
                    new Uri("http://206.72.115.176/SgarbiMix/" + file),
                    new Uri(baseUri + file, UriKind.Relative))));

            for (int i = 0; i < 20; i++)
            {
                StartDownload(TransferQueue.Dequeue());
            }
        }

        private void StartDownload(BackgroundTransferRequest btr)
        {
            var tm = new TransferMonitor(btr);
            tm.Complete += tm_Complete;
            Downloads.Add(tm);
            tm.RequestStart();
        }

        void tm_Complete(object sender, BackgroundTransferEventArgs e)
        {
            if (TransferQueue.Count == 0) return; //no new transfers to add

            if (e.Request.TransferStatus == TransferStatus.Completed &&
                e.Request.TransferError == null)
            {
                //All ok
                BackgroundTransferService.Remove(e.Request);
                StartDownload(TransferQueue.Dequeue());
            }

        }
    }
}
