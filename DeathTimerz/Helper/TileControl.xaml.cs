using System.Windows.Controls;

namespace DeathTimerz.Helper
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
