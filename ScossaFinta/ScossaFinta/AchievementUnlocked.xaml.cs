using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ScossaFinta
{
	public partial class AchievementUnlocked : UserControl
	{
        public string AchievementText
        {
            get { return AchievementTextBlock.Text; }
            set { AchievementTextBlock.Text = value; }
        }

        public void Appear()
        {
            VisualStateManager.GoToState(this, "Visible", true);
        }

		public AchievementUnlocked()
		{
			// Required to initialize variables
			InitializeComponent();
		}
	}
}