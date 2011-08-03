using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace IDecide.Model
{
    [DataContract]
    public class ChoiceGroup
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public IEnumerable<string> Choices { get; set; }
        [DataMember]
        public bool IsSelected { get; set; }
        [DataMember]
        public bool IsDefault { get; set; }

        public ChoiceGroup()
        {
            this.IsSelected = false;
            this.IsDefault = false;
            Choices = new List<string>();
        }
    }
}
