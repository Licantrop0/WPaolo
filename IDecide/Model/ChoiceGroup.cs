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
using System.Collections.Generic;

namespace IDecide.Model
{
    public class ChoiceGroup
    {
        public string Name { get; set; }
        public List<string> Choices { get; set; }
        public bool IsSelected { get; set; }
    }
}
