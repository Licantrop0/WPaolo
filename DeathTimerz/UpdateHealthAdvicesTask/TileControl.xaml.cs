using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace UpdateHealthAdvicesTask
{
    public partial class TileControl : UserControl
    {
        public TileControl()
        {
            InitializeComponent();
            TileContent.Text = HealthAdvices.HealthAdvice.GetAdviceOfTheDay();
        }

        internal void Update(IsolatedStorageFileStream file)
        {
            // Need to call these, otherwise the contents aren’t rendered correctly.
            this.Measure(new Size(336, 336));
            this.Arrange(new Rect(0, 0, 336, 336));

            var wbmp = new WriteableBitmap(336, 336);
            wbmp.Render(this, null);
            wbmp.Invalidate();
            wbmp.SaveJpeg(file, 336, 336, 0, 80);
        }
    }
}
