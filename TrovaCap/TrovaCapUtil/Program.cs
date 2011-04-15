using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using CapUtil;
using TrovaCapUtil;


namespace CAPUtil
{
    class Program
    {
        static string Normalize(string name)
        {
            if (name == "")
                return name;

            // seprate in 3 words
            name = name.Replace("-", " - ");

            // first of all divide in several words, each words must have first letter capitalized
            // all other letters NOT capitalized
            
            List<string> words = name.Split(' ').ToList();
            
            // words.ForEach(s => { s = s.ToLower(); char.ToUpper(s[0]); });   //LAMBDA EXPRESSION = USELESS!!!
            int i;
            for (i = 0; i < words.Count; i++)
            {
                words[i] = words[i].ToLower();
                if(words[i].Length > 1)
                    words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1);

                int nApixIndex = words[i].IndexOf("'");
                if (nApixIndex != -1 && nApixIndex != words[i].Length - 1)
                    words[i] = words[i].Substring(0, nApixIndex + 1) + char.ToUpper(words[i][nApixIndex + 1]) + words[i].Substring(nApixIndex + 2, words[i].Length - (nApixIndex + 2));   
            }

            // gli articoli, le preposizioni vanno scritti maiuscoli, non vale per la prima e l'ultima parola
            i = 1;
            while (i < words.Count - 1)
            {
                switch (words[i])
                {
                    // articoli determinativi
                    case "Il": words[i] = "il"; break;
                    case "Lo": words[i] = "lo"; break;
                    case "La": words[i] = "la"; break;
                    case "L'": words[i] = "l'"; break;
                    case "Gli": words[i] = "gli"; break;
                    case "Le": words[i] = "le"; break;
                    case "I": words[i] = "i"; break;   // si confonde col numero romano (I)

                    // preposizioni semplici
                    case "Di": words[i] = "di"; break;
                    case "A": words[i] = "a"; break;
                    case "Da": words[i] = "da"; break;
                    case "In": words[i] = "in"; break;
                    case "Con": words[i] = "con"; break;
                    case "Su": words[i] = "su"; break;
                    case "Per": words[i] = "per"; break;
                    case "Tra": words[i] = "tra"; break;
                    case "Fra": words[i] = "fra"; break;

                    // preposizione articolate
                    case "Del": words[i] = "del"; break;
                    case "Dello": words[i] = "dello"; break;
                    case "Della": words[i] = "della"; break;
                    case "Dei": words[i] = "dei"; break;
                    case "Degli": words[i] = "degli"; break;
                    case "Delle": words[i] = "delle"; break;
                    case "De": words[i] = "de"; break;
                    case "De'": words[i] = "de'"; break;
                    case "Al": words[i] = "al"; break;
                    case "Allo": words[i] = "allo"; break;
                    case "Alla": words[i] = "alla"; break;
                    case "Ai": words[i] = "ai"; break;
                    case "Agli": words[i] = "agli"; break;
                    case "Alle": words[i] = "alle"; break;
                    case "Dal": words[i] = "dal"; break;
                    case "Dallo": words[i] = "dallo"; break;
                    case "Dalla": words[i] = "dalla"; break;
                    case "Dai": words[i] = "dai"; break;
                    case "Dagli": words[i] = "dagli"; break;
                    case "Dalle": words[i] = "dalle"; break;
                    case "Sul": words[i] = "sul"; break;
                    case "Sui": words[i] = "sui"; break;

                    default: break;
                }

                i++;
            }

            // ogni parola puo' essere un numero romano
            for (i = 0; i < words.Count; i++)
            {
                switch (words[i])
                {
                    case "Ii": words[i] = "II"; break;
                    case "Iii": words[i] = "III"; break;
                    case "Iv": words[i] = "IV"; break;
                    case "Vi": words[i] = "VI"; break;
                    case "Vii": words[i] = "VII"; break;
                    case "Viii": words[i] = "VIII"; break;
                    case "Ix": words[i] = "IX"; break;
                    case "Xi": words[i] = "XI"; break;
                    case "Xii": words[i] = "XII"; break;
                    case "Xiii": words[i] = "XIII"; break;
                    case "Xiv": words[i] = "XIV"; break;
                    case "Xv": words[i] = "XV"; break;
                    case "Xvi": words[i] = "XVI"; break;
                    case "Xvii": words[i] = "XVII"; break;
                    case "Xviii": words[i] = "XVIII"; break;
                    case "Xix": words[i] = "XIX"; break;
                    case "Xx": words[i] = "XX"; break;
                    case "Xxi": words[i] = "XI"; break;
                    case "Xxii": words[i] = "XXII"; break;
                    case "Xxiii": words[i] = "XXIII"; break;
                    case "Xxiv": words[i] = "XXIV"; break;
                    case "Xxv": words[i] = "XXV"; break;
                    case "Xxvi": words[i] = "XXVI"; break;
                    case "Xxvii": words[i] = "XXVII"; break;
                    case "Xxviii": words[i] = "XXVIII"; break;
                    case "Xxix": words[i] = "XXIX"; break;
                    case "Xxx": words[i] = "XXX"; break;
                    case "Xxxi": words[i] = "XXXI"; break;
                    default: break;
                }
            }

            string sReturn = words[0];

            if (words.Count > 1)
                for (i = 1; i < words.Count; i++)
                    sReturn += " " + words[i];

            // sostuire le preposizioni apostrofate
            sReturn = sReturn.Replace("D'", "d'");
            sReturn = sReturn.Replace("Dell'", "dell'");
            sReturn = sReturn.Replace("All'", "all'");
            sReturn = sReturn.Replace("Dall'", "dall'");
            sReturn = sReturn.Replace("Sull'", "sull'");

            return sReturn;
        }

        static void Main(string[] args)
        {
            // TENTATIVE 3 (DB aggiornato)
            var _capDB = new SortedList<string, Comune>(new MyStringComparer());

            using (var tr = new StreamReader("DB3.txt", Encoding.Default))
            {
                while (!tr.EndOfStream)
                {
                    var words = tr.ReadLine().Split('|');

                    /*if (words[1] == "CASTELLABATE")
                    {
                        int pippo = 0;
                        pippo++;
                    }*/

                    string sProvincia = words[0];
                    string sComune1 = Normalize(words[1]);
                    string sComune2 = Normalize(words[2]);
                    string sFrazione1 = Normalize(words[3]);
                    string sFrazione2 = Normalize(words[4]);
                    string sDug = Normalize(words[5]);
                    string sStrada = Normalize(words[6]);
                    string sParita = words[7];
                    string sNDal = words[8];
                    string sEsponenteDal = words[9];
                    string sNAl = words[10];
                    string sEsponenteAl = words[11];
                    string sColore = words[12];
                    string sCAP = words[13];
                    string sIndirizzo = "";
                    string sCivico = "";

                    Console.WriteLine(sProvincia + " " + sComune1 + " " + sComune2 + " " + sFrazione1 + " " + sFrazione2 + " " + sDug + " " + sStrada + " " + sParita + " " + sNDal + " " +
                                      sEsponenteDal + " " + sNAl + " " + sEsponenteAl + " " + sColore + " " + sCAP);

                   
                    
                    // parse indirizzo se necessario
                    if (sStrada != "")
                    {
                        sIndirizzo = sDug + ' ' + sStrada;
                        sCivico = "";

                        if (sParita != "")
                        {
                            if (sParita == "P" || sParita == "D")
                            {
                                string sParitaEx = sParita == "P" ? "pari" : "dispari";
                                sCivico = "N " + sParitaEx;

                                if (sColore == "R")
                                    sCivico += " rossi ";

                                sCivico += " dal " + sNDal;
                                if (sEsponenteDal != "")
                                    sCivico += "/" + sEsponenteDal;

                                sCivico += " al " + sNAl;
                                if (sEsponenteAl != "")
                                    sCivico += "/" + sEsponenteAl;
                            }
                            else
                            {
                                decimal kmDal = Convert.ToInt32(sNDal) / 1000;
                                decimal kmAl = Convert.ToInt32(sNAl) / 1000;

                                sCivico += "dal km " + kmDal.ToString() + " al km " + kmAl.ToString();
                            }

                            sIndirizzo += ", " + sCivico;
                        }
                    }

                    // si accorpano le frazioni bilingui
                    string sFrazione = sFrazione2 == "" ? sFrazione1 : sFrazione1 + " - " + sFrazione2;

                    CAPRecord newRecord = new CAPRecord(sFrazione, sIndirizzo, sCAP);

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

                        if (sComune2 != "" && sComune2 != sComune1)
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
            using (var tout = new StreamWriter("DB3out.txt", false, Encoding.Default))
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

            // write just comuni names to another file
            using (var tout2 = new StreamWriter("Comuni.txt", false, Encoding.Default))
            {
                
                int nComuni = _capDB.Count;

                foreach (var item in _capDB)
                {
                    tout2.Write(item.Value.comuneID + "|");
                }
            }

        }

    }
}


        // TENTATIVE 1

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

            // TENTATIVE 2
            /*
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
             * */

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