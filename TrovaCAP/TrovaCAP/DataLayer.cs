using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Resources;
using Microsoft.Phone.Shell;

namespace TrovaCAP
{
    static class DataLayer
    {
        public static Comune[] Comuni { get; set; }
        public static string[] ComuniNames
        {
            get
            {
                if (!PhoneApplicationService.Current.State.ContainsKey("comuni_names"))
                    PhoneApplicationService.Current.State.Add("comuni_names", null);

                return (string[])PhoneApplicationService.Current.State["comuni_names"];
            }
            set
            {
                PhoneApplicationService.Current.State["comuni_names"] = value;
            }
        }

        public static void LoadComuniNames()
        {
            var resource = Application.GetResourceStream(new Uri("Comuni.txt", UriKind.Relative));
            using (var tr = new StreamReader(resource.Stream))
            {
                ComuniNames = tr.ReadToEnd().Split('|');
            }
        }

        public static void LoadDBAsync()
        {
            var bw = new BackgroundWorker();
            bw.DoWork += (sender1, e1) =>
            {
                DataLayer.ReadAndParseDataBase();
                //DataLayer.Deserialize();
            };
            bw.RunWorkerAsync();
        }

        private static void ReadAndParseDataBase()
        {
            var resource = Application.GetResourceStream(new Uri("DB3out.txt", UriKind.Relative));
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