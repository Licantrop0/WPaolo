using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;


namespace TrovaCAP
{
    class CAPDB
    {
        public Dictionary<string, Dictionary<string, Comune>> Province { get; set; }
        public Dictionary<string, Comune> Comuni { get; set; }
        public Dictionary<string, CAP> CAPS { get; set; }

        public CAPDB(Dictionary<string, Dictionary<string, Comune>> province, Dictionary<string, Comune> comuni, Dictionary<string, CAP> caps)
        {
            Province = province;
            Comuni = comuni;
            CAPS = caps;
        }

        public CAPDB() {}
    }

    class CAP
    {
        public string CAPString { get; set; }

        public CAP(string cap)
        {
            CAPString = cap;
        }

        public CAP() {}
    }

    class CAPRecord
    {
        public string Frazione { get; set; }
        public string Indirizzo { get; set; }
        public string Civico { get; set; }
        public CAP Cap { get; set; }

        public CAPRecord(string fr, string ind, string civ)
        {
            Frazione = fr;
            Indirizzo = ind;
            Civico = civ;
        }

        public CAPRecord() {}
    }

    class Comune
    {
        public string ComuneID { get; set; }
        public List<CAPRecord> CapRecords { get; set; }     // bisogna trasformarla in un vettore!

        public Comune() {}
    }
}
