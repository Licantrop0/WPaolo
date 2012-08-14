using System.Runtime.Serialization;

namespace NascondiChiappe.Model
{
    [DataContract]
    public class OldAlbum
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string DirectoryName { get; set; }
    }
}
