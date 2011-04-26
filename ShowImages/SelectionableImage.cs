using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.IO;
using System.Runtime.Serialization;

namespace ShowImages
{
    [DataContract]
    public class SelectionableImage
    {
        [DataMember]
        public string Url { get; set; }
        [DataMember]
        public bool IsSelected { get; set; }
        [DataMember]
        public string Name { get; set; }

        public SelectionableImage(string url, string name)
        {
            Url = url;
            IsSelected = false;
        }
    }
}
