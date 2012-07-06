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

namespace Scudetti.Model
{
    [DataContract]
    public class Shield
    {
        [DataMember]
        public string Name { get; set; }
        
        [DataMember]
        public string Image { get; set; }
        
        [DataMember]
        public int Level { get; set; }

        [DataMember]
        public bool IsValidated { get; set; }

        public Shield(string name, int level, string image)
        {
            Name = name;
            Image = image;
            Level = level;
            IsValidated = false;
        }
    }
}
