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
using System.Linq;
using System.Runtime.Serialization;

namespace MediaEsami
{

    [DataContract]
    public class Exam
    {
        public int Id { get { return Name.GetHashCode(); } }

        public double Credits { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int? Mark { get; set; }

        public Exam(string name, double credits)
        {
            Name = name;
            Credits = credits;
            Mark = null;
        }
    }
}