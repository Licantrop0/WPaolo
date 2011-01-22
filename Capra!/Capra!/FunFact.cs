﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Capra
{
    public class FunFact
    {
        public string Type { get; set; }
        public string Text { get; set; }

        public FunFact(string type, string text)
        {
            Type = type;
            Text = text;
        }
    }
}
