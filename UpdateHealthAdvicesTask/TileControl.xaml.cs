using System.Windows.Controls;

namespace UpdateHealthAdvicesTask
{
    public partial class TileControl : UserControl
    {
        public TileControl(string text)
        {
            InitializeComponent();
            TileContent.Text = text;
        }
    }
}
