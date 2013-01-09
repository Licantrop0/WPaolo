using Microsoft.Phone.Shell;
using System;
using System.IO.IsolatedStorage;
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

        public StandardTileData ToTile()
        {
            // Need to call these, otherwise the contents aren’t rendered correctly.
            this.Measure(new Size(336, 336));
            this.Arrange(new Rect(0, 0, 336, 336));

            var bmp = new WriteableBitmap(336, 336);
            bmp.Render(this, null);
            bmp.Invalidate();

            using (var iss = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var stm = iss.CreateFile("/Shared/ShellContent/LiveTileIcon.jpg"))
                {
                    bmp.SaveJpeg(stm, 336, 336, 0, 80);
                    stm.Close();
                }
            }

            return new StandardTileData()
            {
                BackBackgroundImage = new Uri("isostore:/Shared/ShellContent/LiveTileIcon.jpg", UriKind.Absolute)
            };
        }

    }
}
