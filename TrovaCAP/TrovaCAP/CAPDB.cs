using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;


namespace TrovaCAP
{
    public class CAPRecord
    {
        public string Frazione { get; set; }
        public string Indirizzo { get; set; }
        public string Cap { get; set; }

        public CAPRecord(string fr, string ind, string cap)
        {
            Frazione = fr;
            Indirizzo = ind;
            Cap = cap;
        }

        public CAPRecord() { }
    }

    public class Comune
    {
        public string ComuneID { get; set; }
        public CAPRecord[] CapRecords { get; set; }

        public Comune(string id, CAPRecord[] records)
        {
            ComuneID = id;
            CapRecords = records;
        }
    }

    public class CapDB
    {
        public Comune[] Comuni { get; set; }

        public CapDB(Comune[] comuni)
        {
            Comuni = comuni;
        }
    }
}
