using Microsoft.Phone.BackgroundTransfer;
using Microsoft.Phone.Controls;
using System;
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
        const string baseUri = "shared/transfers/";
        static IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();
        public ObservableCollection<TransferMonitor> Downloads { get; set; }
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

            var existings = isf.GetFileNames(baseUri + "*.wav");
            var differences = sounds.Select(s => s.File)
                .Except(existings.Select(f => HttpUtility.UrlDecode(f)))
                .Take(5).Concat(new[] {"Sounds.xml"});

            foreach (var file in differences)
            {
                var btr = new BackgroundTransferRequest(
                    new Uri("http://206.72.115.176/SgarbiMix/" + file),
                    new Uri(baseUri + file, UriKind.Relative));

                btr.TransferStatusChanged += btr_TransferStatusChanged;

                var tm = new TransferMonitor(btr);

                tm.Failed += tm_Failed;
                Downloads.Add(tm);
                tm.RequestStart();
            }
        }

        void btr_TransferStatusChanged(object sender, BackgroundTransferEventArgs e)
        {
            if (e.Request.TransferError != null)
            {
                MessageBox.Show(e.Request.TransferError.Message);
            }

        }

        void tm_Failed(object sender, BackgroundTransferEventArgs e)
        {
            
        }
    }
}
