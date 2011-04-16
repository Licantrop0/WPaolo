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
using System.Runtime.Serialization;

namespace ScossaFinta
{
    [DataContract]
    public class Stats
    {
        [DataMember]
        public uint NumeroDiScosse { get; set; }

        public Stats(uint nScosse)
        {
            NumeroDiScosse = nScosse;
            //TO DO
            //pensare se è il caso di aggiungere achievements sbloccati da altri parametri,
            //per esempio il numero medio di scosse al giorno... Oppure è una statistica che può
            //essere comunque inserita anche se non collegata ad un achievement.
        }
    }
}
