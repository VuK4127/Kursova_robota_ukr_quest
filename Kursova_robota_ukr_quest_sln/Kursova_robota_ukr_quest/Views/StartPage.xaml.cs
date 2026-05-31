using System.Windows;
using System.Windows.Controls;

namespace Kursova_robota_ukr_quest.Views
{
    public partial class StartPage : Page
    {
        private bool _isDark = false;

        public StartPage() => InitializeComponent();

        private void Start_Click(object sender, RoutedEventArgs e) =>
            NavigationService.Navigate(new ModePage());

        private void Exit_Click(object sender, RoutedEventArgs e) =>
            Application.Current.Shutdown();

        private void Profile_Click(object sender, RoutedEventArgs e) =>
            NavigationService.Navigate(new ProfilePage());

        private void ThemeToggle_Click(object sender, RoutedEventArgs e)
        {
            _isDark = !_isDark;
            ThemeHelper.ApplyTheme(_isDark ? "DarkTheme.xaml" : "LightTheme.xaml");
            ThemeIcon.SetDark(_isDark);
        }
    }
}
