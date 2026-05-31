using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Kursova_robota_ukr_quest.Models;
using Kursova_robota_ukr_quest.Services;

namespace Kursova_robota_ukr_quest.Views
{
    public partial class ProfilePage : Page
    {
        private readonly ProfileService _profileService = new();

        public ProfilePage()
        {
            InitializeComponent();
            NicknameBox.Text = UserProfile.Nickname;

            AvatarBrush.ImageSource = !string.IsNullOrEmpty(UserProfile.AvatarPath)
                ? new BitmapImage(new Uri(UserProfile.AvatarPath))
                : ThemeHelper.GetDefaultAvatar();
        }

        private void ChangeAvatar_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Filter = "Зображення|*.png;*.jpg;*.jpeg;*.bmp" };
            if (dialog.ShowDialog() == true)
            {
                UserProfile.AvatarPath  = dialog.FileName;
                AvatarBrush.ImageSource = new BitmapImage(new Uri(dialog.FileName));
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string nick = NicknameBox.Text.Trim();
            if (string.IsNullOrEmpty(nick))
            { MessageBox.Show("Нікнейм не може бути порожнім."); return; }
            if (nick.Length < 2 || nick.Length > 30)
            { MessageBox.Show("Нікнейм: від 2 до 30 символів."); return; }

            UserProfile.Nickname = nick;
            _profileService.Persist();
            LogService.Write(UserProfile.Login, $"оновив профіль: нікнейм = {nick}");
            MessageBox.Show("Збережено!");
            NavigationService.GoBack();
        }

        private void Back_Click(object sender, RoutedEventArgs e) =>
            NavigationService.GoBack();
    }
}
