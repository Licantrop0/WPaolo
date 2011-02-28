using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


namespace CAPUtil
{
    [Serializable()]    //Set this attribute to all the classes that want to serialize
    // classe contenitore da serializzare  :-)
    class CAPDB : ISerializable
    {
        public SortedList<string, SortedList<string, Comune>> Province;
        public SortedList<string, Comune> Comuni;
        public SortedList<string, CAP> CAPS;

        public CAPDB(SortedList<string, SortedList<string, Comune>> province, SortedList<string, Comune> comuni, SortedList<string, CAP> caps)
        {
            Province = province;
            Comuni = comuni;
            CAPS = caps;
        }

        public CAPDB(SerializationInfo info, StreamingContext ctxt)
        {
            Province = (SortedList<string, SortedList<string, Comune>>)info.GetValue("Province", typeof(SortedList<string, SortedList<string, Comune>>));
            Comuni = (SortedList<string, Comune>)info.GetValue("Comuni", typeof(SortedList<string, Comune>));
            CAPS = (SortedList<string, CAP>)info.GetValue("CAPS", typeof(SortedList<string, CAP>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Province", Province);
            info.AddValue("Comuni", Comuni);
            info.AddValue("CAPS", CAPS);
        }
    }

    [Serializable()]
    class CAP
    {
        public string CAPstring;

        public CAP(string cap)
        {
            CAPstring = cap;
        }
    }

    [Serializable()]
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
            civico = civ;
        }
    }

    [Serializable()]
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
            stream.Close();
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
