using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

//using CAPs = List<string>;
//using Frazione = SortedList<string, string>
//using Comune = SortedList<string, List<CAP.CAPRecord>>;

namespace CAP
{
    class CAP
    {
        public string CAPstring;

        public CAP(string cap)
        {
            CAPstring = cap;
        }
    }

    // per adesso, poi vediamo se si può normalizzare anche questo!
    class CAPRecord
    {
        public string frazione;
        public string indirizzo;
        public string civico;
        public CAP cap;

        public CAPRecord(string fr, string ind, string civ)
        {
            frazione = fr;
            indirizzo = ind;
            if (indirizzo == " ()")
            {
                int a = 0;
            }
            civico = civ;
        }
    }

    class Comune
    {
        public string comuneID;
        public List<CAPRecord> capRecords;   // per adesso, poi può essere destrutturato...
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            SortedList<string, SortedList<string, Comune>> dProvince = new SortedList<string,SortedList<string,Comune>>();
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

                //debugWriter.WriteLine(sComune1 + " " + sComune2 + " " + sFrazione1 + " " + sFrazione2 + " " + sToponimo1 + " " + sToponimo2 + " " + sDaugt + "\n");

                string sCap = Normalize(words[9]);

                // insert CAP in CAP SortedList if necessary
                if (!dCAPS.ContainsKey(sCap))
                {
                    dCAPS.Add(sCap, new CAP(sCap));
                }

                if (sCap == "" || sCap == "0 al 1" || sCap == "0 al 2" || sCap == "0 al 6" || sCap == "0 al 7")
                {
                    int a = 0;
                    a++;
                }

                CAPRecord newRecord = new CAPRecord(sFrazione, sToponimo, sCivico);
                newRecord.cap = dCAPS[sCap];    // devo testare che sia un reference value, no, non lo è...

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
                    if (sProvincia != "BZ" && sProvincia != "BG" && sProvincia != "GR")
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
        }

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
    }
}
