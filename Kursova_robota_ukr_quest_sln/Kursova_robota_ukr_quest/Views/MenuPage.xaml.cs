using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Kursova_robota_ukr_quest.Models;
using Kursova_robota_ukr_quest.Services;

namespace Kursova_robota_ukr_quest.Views
{
    public partial class MenuPage : Page
    {
        private bool _darkMode = false;

        public MenuPage() => InitializeComponent();

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            TopNickname.Text = UserProfile.Nickname;
            TopAvatarBrush.ImageSource = !string.IsNullOrEmpty(UserProfile.AvatarPath)
                ? new BitmapImage(new Uri(UserProfile.AvatarPath))
                : ThemeHelper.GetDefaultAvatar();

            AdminBtn.Visibility = UserProfile.Role == "Admin"
                ? Visibility.Visible : Visibility.Collapsed;

            RoleBadge.Text = UserProfile.Role == "Admin" ? "👑 Адміністратор" : "👤 Користувач";

            LogService.Write(UserProfile.Login, "відкрив головне меню");
        }

        private void Play_Click(object sender, RoutedEventArgs e) =>
            NavigationService.Navigate(new ModePage());

        private void Admin_Click(object sender, RoutedEventArgs e)
        {
            if (UserProfile.Role != "Admin")
            {
                MessageBox.Show("Доступ заборонено.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            NavigationService.Navigate(new AdminPage());
        }

        private void Profile_Click(object sender, RoutedEventArgs e) =>
            NavigationService.Navigate(new ProfilePage());

        private void ThemeToggle_Click(object sender, RoutedEventArgs e)
        {
            _darkMode = !_darkMode;
            ThemeHelper.ApplyTheme(_darkMode ? "DarkTheme.xaml" : "LightTheme.xaml");
            ThemeIcon.SetDark(_darkMode);
        }

        private void Exit_Click(object sender, RoutedEventArgs e) =>
            Application.Current.Shutdown();
    }
}
