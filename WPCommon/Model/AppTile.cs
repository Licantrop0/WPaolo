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

namespace WPCommon.Model
{
    public class AppTile
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public Uri Thumbnail { get; private set; }

        public AppTile(Guid id, string name, Uri thumbnail)
        {
            Id = id;
            Name = name;
            Thumbnail = thumbnail;
        }
    }
}
