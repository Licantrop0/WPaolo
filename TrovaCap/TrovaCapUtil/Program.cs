using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using CapUtil;


namespace CAPUtil
{
    class Program
    {
        /*
        static string Normalize(string s)
        {
            string sReturn;
            if (s == " '")
            {
                sReturn = "";
            }
            else
            {
                sReturn = s.Remove(0, 2);
                sReturn = sReturn.Remove(sReturn.Length - 1, 1);
            }

            return sReturn;
        }
         * */

        static void Main(string[] args)
        {
            /*SortedList<string, SortedList<string, Comune>> dProvince = new SortedList<string,SortedList<string,Comune>>();
            SortedList<string, Comune> dComuni = new SortedList<string, Comune>();
            SortedList<string, CAP> dCAPS = new SortedList<string, CAP>();

            TextReader tr = new StreamReader("DB2.txt");
            string tmp;

            TextWriter debugWriter = new StreamWriter("debugApix.txt");

            int nDebug = 0;
            // read a line of text
            while ((tmp = tr.ReadLine()) != null)
            {
                nDebug++;
                if(nDebug % 1000 == 0)
                    System.Console.WriteLine(nDebug.ToString());

                if (nDebug == 68832)
                {
                    int b = 0;
                    b++;
                }

                // read a line on ASCII file, separate strings on comma char
                string[] words = tmp.Split(',');

                string sProvincia = Normalize(words[0]);

                string sComune1 = Normalize(words[1]);
                string sComune2 = Normalize(words[2]);

                string sFrazione1 = Normalize(words[3]);
                string sFrazione2 = Normalize(words[4]);
                string sFrazione  = sFrazione2 == "" ? sFrazione1 : sFrazione1 + " - " + sFrazione2;

                string sToponimo1 = Normalize(words[5]);
                string sToponimo2 = Normalize(words[6]);
                string sToponimo = sToponimo2 == "" ? sToponimo1 : sToponimo1 + " - " + sToponimo2;

                string sDaugt = Normalize(words[7]);
                sToponimo = sToponimo == "" ? sToponimo : sToponimo + " (" + sDaugt + ")";

                string sCivico = Normalize(words[8]);

                string sCap = Normalize(words[9]);

                // insert CAP in CAP SortedList if necessary
                if (!dCAPS.ContainsKey(sCap))
                {
                    dCAPS.Add(sCap, new CAP(sCap));
                }

                CAPRecord newRecord = new CAPRecord(sFrazione, sToponimo, sCivico);
                newRecord.cap = dCAPS[sCap];

                // insert in province SortedList if not already inserted
                if (!dProvince.ContainsKey(sProvincia))
                {
                    dProvince.Add(sProvincia, new SortedList<string, Comune>());
                }

                // parse Comune1, Comune2 if necessary
                if (sComune2 == "")
                {
                    sComune1 = sComune1 + " (" + sProvincia + ")";
                }
                else
                {
                    if (sProvincia != "BZ" && sProvincia != "BG" && sProvincia != "GR" && sProvincia != "St" && sProvincia != "TN" && sProvincia != "TO")
                    {
                        int pippo = 0;
                    }
                    string sTmp = sComune1;
                    sComune1 = sTmp + " - " + sComune2 + " (" + sProvincia + ")";
                    sComune2 = sComune2 + " - " + sTmp + " (" + sProvincia + ")";
                }
                
                // insert into comuni SortedList if not already inserted, if comune name is bilungual insert two separate records
                if (!dComuni.ContainsKey(sComune1))
                {
                    if (sComune2 != "")
                    {
                        Comune c2 = new Comune();
                        c2.comuneID = sComune2;
                        c2.capRecords = new List<CAPRecord>();
                        dComuni.Add(c2.comuneID, c2);

                        dProvince[sProvincia].Add(c2.comuneID, c2);
                    }

                    Comune c1 = new Comune();
                    c1.comuneID = sComune1;
                    c1.capRecords = new List<CAPRecord>();
                    dComuni.Add(sComune1, c1);

                    dProvince[sProvincia].Add(c1.comuneID, c1);
                }

                // insert CAP record into both indexes
                if (sComune2 != "")
                {
                    //dProvince[sProvincia][sComune2].capRecords.Add(newRecord);
                    dComuni[sComune2].capRecords.Add(newRecord);    // lo aggiunge automaticamente anche sopra...sono tutti puntatori, o come dicono gli sharpisti "reference value"
                }

                //dProvince[sProvincia][sComune1].capRecords.Add(newRecord);
                dComuni[sComune1].capRecords.Add(newRecord);    // lo aggiunge automaticamente anche sopra...sono tutti puntatori, o come dicono gli sharpisti "reference value"
            }

            CAPDB capDB = new CAPDB(dProvince, dComuni, dCAPS);

            Stream stream = File.Open("CAPDB.osl", FileMode.Create);
            BinaryFormatter bformatter = new BinaryFormatter();

            Console.WriteLine("Writing CAP DB Information");
            bformatter.Serialize(stream, capDB);
            stream.Close();*/

            //private void ReadAndParseDataBase()

            var _capDB = new SortedList<string, Comune>();

            using (var tr = new StreamReader("DB2.txt", Encoding.Default))
            {
                while(!tr.EndOfStream)   
                {
                    var words = tr.ReadLine().Split('|');

                    string sProvincia = words[0];
                    string sComune1 = words[1];
                    string sComune2 = words[2];

                    string sFrazione = string.IsNullOrEmpty(words[4]) ? words[3] : words[3] + " - " + words[4];
                    string sIndirizzo = string.IsNullOrEmpty(words[6]) ? words[5] : words[5] + " - " + words[6];
                    sIndirizzo = string.IsNullOrEmpty(sIndirizzo) ? sIndirizzo : words[7] + " " + sIndirizzo;

                    string sCivico = words[8];
                    string sCap = words[9];

                    if(sCivico != "")
                    {
                        sIndirizzo += " " + sCivico; 
                    }

                    CAPRecord newRecord = new CAPRecord(sFrazione, sIndirizzo, sCap);

                    // parse Comune1, Comune2 if necessary
                    sComune1 = sComune1 + " (" + sProvincia + ")";

                    if (sComune2 != "")
                        sComune2 += " (" + sProvincia + ")";

                    // insert into comuni Dictionary if not already inserted, if comune name is bilungual insert two separate records
                    if (!_capDB.ContainsKey(sComune1))
                    {
                        Comune c1 = new Comune();
                        c1.comuneID = sComune1;
                        c1.capRecords = new List<CAPRecord>();
                        _capDB.Add(sComune1, c1);

                        if (sComune2 != "")
                        {
                            Comune c2 = new Comune();
                            c2.comuneID = sComune2;
                            c2.capRecords = new List<CAPRecord>();
                            _capDB.Add(c2.comuneID, c2);
                        }
                    }

                    // insert CAP record into comune index
                    _capDB[sComune1].capRecords.Add(newRecord);

                    if (sComune2 != "")
                    {
                        _capDB[sComune2].capRecords.Add(newRecord);
                    }
                }
            }

            // write data to file
            using (var tout = new StreamWriter("DBout.txt", false, Encoding.Default))
            {
                int nComuni = _capDB.Count;

                tout.WriteLine(nComuni.ToString());

                foreach (var item in _capDB)
                {
                    int nCAPRecords = item.Value.capRecords.Count;
                    string sComune = item.Value.comuneID;

                    tout.WriteLine(sComune + "|" + nCAPRecords);

                    foreach (var cr in item.Value.capRecords)
                    {
                        tout.WriteLine(cr.frazione + "|" + cr.indirizzo + "|" + cr.cap);
                    }
                }
            }
        }

        
    }
}
