using System.Windows;
using System.Windows.Controls;

namespace Kursova_robota_ukr_quest.Controls
{
    public partial class ThemeIconControl : UserControl
    {
        public ThemeIconControl()
        {
            InitializeComponent();
        }

        public void SetDark(bool isDark)
        {
            SunCanvas.Visibility = isDark ? Visibility.Collapsed : Visibility.Visible;
            MoonCanvas.Visibility = isDark ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
