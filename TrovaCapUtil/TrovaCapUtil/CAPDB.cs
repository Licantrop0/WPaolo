using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace CapUtil
{

    [Serializable()]
    // per adesso, poi vediamo se si può normalizzare anche questo!
    class CAPRecord
    {
        public string frazione;
        public string indirizzo;
        public string cap;

        public CAPRecord(string fr, string ind, string c)
        {
            frazione = fr;
            indirizzo = ind;
            cap = c;
        }
    }

    [Serializable()]
    class Comune
    {
        public string comuneID;
        public List<CAPRecord> capRecords;   // per adesso, poi può essere destrutturato...
    }
}
