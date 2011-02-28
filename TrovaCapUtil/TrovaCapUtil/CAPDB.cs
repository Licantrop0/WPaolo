using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace CapUtil
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
}
