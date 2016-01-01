using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GalaSoft.MvvmLight;
using TouchColors.DesignMode;
using TouchColors.Helper;
using TouchColors.Model;
using Windows.UI.Xaml.Controls;

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
            if (IsInDesignMode)
            {
                ColorList = DesignData.GetColors();
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
    }
}