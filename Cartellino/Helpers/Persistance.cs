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
using System.IO.IsolatedStorage;

namespace Cartellino.Helpers
{
    public class Persistance
    {
        // larghezza dello schermo
        public const int PORTRAIT_WORKING_AREA_WIDTH = 480;

        public static double HorizontalOffset
        {
            get
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("horizontal_offset"))
                    IsolatedStorageSettings.ApplicationSettings.Add("horizontal_offset", 0d);

                return (double)IsolatedStorageSettings.ApplicationSettings["horizontal_offset"];
            }
            set
            {
                if (HorizontalOffset == value)
                    return;
                IsolatedStorageSettings.ApplicationSettings["horizontal_offset"] = value;
            }
        }

        public static int CurrentPage
        {
            get { return Convert.ToInt32(HorizontalOffset / PORTRAIT_WORKING_AREA_WIDTH); }
        }
    }
}
