using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrovaCAP;
using System.IO;

namespace TrovaCAPWriteSerializedFile
{
    class Program
    {
        static void Main(string[] args)
        {
            ReadAndParseDataBase();
        }

        static void ReadAndParseDataBase()
        {
            Comune[] comuni = null;

            using (var sr = new StreamReader("DBout.txt"))
            {
                int count = int.Parse(sr.ReadLine());
                comuni = new Comune[count];

                for (int i = 0; i < count; i++)
                {
                    string[] words = sr.ReadLine().Split('|');

                    int nRecordCount = int.Parse(words[1]);
                    comuni[i] = new Comune(words[0], new CAPRecord[nRecordCount]);

                    for (int j = 0; j < nRecordCount; j++)
                    {
                        string[] parole = sr.ReadLine().Split('|');
                        comuni[i].CapRecords[j] = new CAPRecord(parole[0], parole[1], parole[2]);
                    }
                }
            }

            CapDB capDB = new CapDB(comuni);

            // serialization
            FileStream fout = new FileStream("DB.ser", FileMode.Create);
            CustomBinarySerializer ser = new CustomBinarySerializer(capDB.GetType());
            ser.WriteObject(fout, comuni);
            fout.Close();
            
        }
    }
}
