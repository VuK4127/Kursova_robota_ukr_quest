using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Kursova_robota_ukr_quest.Models;
using Kursova_robota_ukr_quest.Services;

namespace Kursova_robota_ukr_quest.Views
{
    public partial class LoginPage : Page
    {
        private static readonly AuthService    _authService    = new();
        private static readonly ProfileService _profileService = new();
        private int _failCount   = 0;
        private const int AttemptLimit = 5;

        public LoginPage()
        {
            InitializeComponent();
            _profileService.Restore();
        }

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) TryLogin();
        }

        private void Login_Click(object sender, RoutedEventArgs e) => TryLogin();

        private void GoRegister_Click(object sender, RoutedEventArgs e) =>
            NavigationService.Navigate(new RegisterPage());

        private void TryLogin()
        {
            string login = LoginBox.Text.Trim();
            string pass  = PasswordBox.Password;

            if (string.IsNullOrEmpty(login))
            { ErrorText.Text = "Введіть логін."; return; }
            if (string.IsNullOrEmpty(pass))
            { ErrorText.Text = "Введіть пароль."; return; }

            if (_failCount >= AttemptLimit)
            {
                ErrorText.Text = $"Забагато невдалих спроб ({AttemptLimit}). Перезапустіть програму.";
                LogService.Write(login, "перевищив ліміт спроб входу");
                return;
            }

            var account = _authService.Authenticate(login, pass);
            if (account == null)
            {
                _failCount++;
                AttemptsText.Text = $"Невдала спроба {_failCount}/{AttemptLimit}";
                ErrorText.Text    = "Неправильний логін або пароль.";
                LogService.Write(login, $"невдала спроба входу ({_failCount})");
                return;
            }

            UserProfile.Login    = account.Login;
            UserProfile.Role     = account.Role;
            UserProfile.Nickname = account.Login;
            _profileService.Restore();
            UserProfile.Login = account.Login;
            UserProfile.Role  = account.Role;

            LogService.Write(account.Login, $"увійшов як {account.Role}");
            NavigationService.Navigate(new MenuPage());
        }
    }
}
