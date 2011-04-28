using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Resources;
using Microsoft.Phone.Shell;

namespace TrovaCAP.Data
{
    static class DataLayer
    {
        public static Comune[] Comuni { get; set; }
        public static string[] ComuniNames { get; set; }

        public static void LoadComuniNames()
        {
            var resource = Application.GetResourceStream(new Uri("Data/Comuni.txt", UriKind.Relative));
            using (var tr = new StreamReader(resource.Stream))
            {
                ComuniNames = tr.ReadToEnd().Split('|');
            }
        }

        public static void LoadDBAsync()
        {
            var bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerAsync();
        }

        static void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            var resource = Application.GetResourceStream(new Uri("Data/DB3out.txt", UriKind.Relative));
            using (var tr = new StreamReader(resource.Stream))
            {
                int count = int.Parse(tr.ReadLine());
                Comuni = new Comune[count];

                for (int i = 0; i < count; i++)
                {
                    string[] words = tr.ReadLine().Split('|');

                    int nRecordCount = int.Parse(words[1]);
                    Comuni[i] = new Comune(words[0], new CAPRecord[nRecordCount]);

                    for (int j = 0; j < nRecordCount; j++)
                    {
                        string[] parole = tr.ReadLine().Split('|');
                        Comuni[i].CapRecords[j] = new CAPRecord(parole[0], parole[1], parole[2]);
                    }
                }
            }
        }

        /*private static void Deserialize()
        {
            StreamResourceInfo sri = Application.GetResourceStream(new Uri("DB3.ser", UriKind.Relative));
            using (var fin = new StreamReader(sri.Stream))
            {
                CustomBinarySerializer ser2 = new CustomBinarySerializer(typeof(CapDB));
                Comuni = ser2.ReadObject(sri.Stream) as Comune[];
            }
        }*/
    }
}