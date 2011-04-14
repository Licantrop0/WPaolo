using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Resources;
using Microsoft.Phone.Shell;

namespace TrovaCAP
{
    static class DataLayer
    {
        public static Comune[] Comuni
        {
            get
            {
                if (!PhoneApplicationService.Current.State.ContainsKey("comuni"))
                    PhoneApplicationService.Current.State.Add("comuni", null); //Valore di default

                return (Comune[])PhoneApplicationService.Current.State["comuni"];
            }
            set
            {
                if (Comuni != value)
                    PhoneApplicationService.Current.State["comuni"] = value;
            }
        }

        public static string[] ComuniNames
        {
            get
            {
                if (!PhoneApplicationService.Current.State.ContainsKey("comuni_names"))
                    PhoneApplicationService.Current.State.Add("comuni_names", null); //Valore di default

                return (string[])PhoneApplicationService.Current.State["comuni_names"];
            }
            set
            {
                if (ComuniNames != value)
                    PhoneApplicationService.Current.State["comuni_names"] = value;
            }
        }


        public static void ReadAndParseDataBase()
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

        public static void LoadComuniNames()
        {
            var resource = Application.GetResourceStream(new Uri("Comuni.txt", UriKind.Relative));

            using (var tr = new StreamReader(resource.Stream))
            {
                ComuniNames = tr.ReadToEnd().Split('|');
            }
        }

        public static void Deserialize()
        {
            StreamResourceInfo sri = Application.GetResourceStream(new Uri("DB3.ser", UriKind.Relative));
            using (var fin = new StreamReader(sri.Stream))
            {
                CustomBinarySerializer ser2 = new CustomBinarySerializer(typeof(CapDB));
                Comuni = ser2.ReadObject(sri.Stream) as Comune[];
            }
        }

    }
}
