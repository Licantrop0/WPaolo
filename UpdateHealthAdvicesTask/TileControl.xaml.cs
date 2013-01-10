using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace UpdateHealthAdvicesTask
{
    public partial class TileControl : UserControl
    {
        public TileControl(string text)
        {
            InitializeComponent();
            TileContent.Text = text;
        }

        public WriteableBitmap ToTile()
        {
            // Need to call these, otherwise the contents aren’t rendered correctly.
            this.Measure(new Size(336, 336));
            this.Arrange(new Rect(0, 0, 336, 336));

            var bmp = new WriteableBitmap(336, 336);
            bmp.Render(this, null);
            bmp.Invalidate();
            return bmp;
        }
    }
}
