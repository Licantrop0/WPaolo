using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Template10.Mvvm;
using Windows.ApplicationModel;
using TouchColors.Helper;
using TouchColors.Model;
using Windows.UI.Xaml.Controls;
using Windows.UI;

namespace TouchColors.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ISpeechHelper _speechHelper;

        public List<NamedColor> ColorList { get; set; }

        private NamedColor _selectedColor;
        public NamedColor SelectedColor
        {
            get { return _selectedColor; }
            set { Set(ref _selectedColor, value); }
        }

        public MainViewModel(ISpeechHelper speechHelper)
        {
            if (DesignMode.DesignModeEnabled)
            {
                ColorList = GetSampleColors();
                SelectedColor = ColorList.First();
                return;
            }

            _speechHelper = speechHelper;

            ColorList = XElement.Load("Data/AllColors.xml").Elements()
                .Select(e => new NamedColor(e.Attribute("name").Value, ColorConverter.FromRgb(e.Attribute("value").Value)))
                .OrderBy(c => c.Luminosity)
                .ToList();
        }

        public void Item_Click(object sender, ItemClickEventArgs e)
        {
            SelectedColor = (NamedColor)e.ClickedItem;
            _speechHelper.Speak(SelectedColor.Name);
        }

        private static List<NamedColor> GetSampleColors()
        {
            return new List<NamedColor>
            {
                new NamedColor("Blue", Colors.Blue),
                new NamedColor("Brown", Colors.Brown),
                new NamedColor("Cornsilk", Colors.Cornsilk),
                new NamedColor("Dark Red", Colors.DarkRed),
                new NamedColor("Green", Colors.Green)
            };
        }
    }
}