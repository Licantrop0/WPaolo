using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace FillTheSquare
{
    public partial class Records : PhoneApplicationPage
    {
        public Records()
        {
            InitializeComponent();
        }

        private void FillRecordsList()
        {
            for (int i = 0; i < Settings.RecordsList.Count; i++)
            {
                //TO DO: aggiungo una riga alla griglia
            }
        }
    }
}