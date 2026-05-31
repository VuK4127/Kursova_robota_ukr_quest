using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Kursova_robota_ukr_quest.Models;
using Kursova_robota_ukr_quest.Services;

namespace Kursova_robota_ukr_quest.Views
{
    public partial class RegisterPage : Page
    {
        private static readonly AuthService _authService = new();

        public RegisterPage() => InitializeComponent();

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) TryRegister();
        }

        private void Register_Click(object sender, RoutedEventArgs e) => TryRegister();

        private void GoLogin_Click(object sender, RoutedEventArgs e) =>
            NavigationService.Navigate(new LoginPage());

        private void TryRegister()
        {
            string nick  = NicknameBox.Text.Trim();
            string login = LoginBox.Text.Trim();
            string pass  = PasswordBox.Password;
            string conf  = ConfirmBox.Password;

            if (string.IsNullOrEmpty(nick))
            { ErrorText.Text = "Введіть нікнейм."; return; }
            if (nick.Length < 2 || nick.Length > 30)
            { ErrorText.Text = "Нікнейм: від 2 до 30 символів."; return; }

            if (string.IsNullOrEmpty(login))
            { ErrorText.Text = "Введіть логін."; return; }
            if (login.Length < 3 || login.Length > 20)
            { ErrorText.Text = "Логін: від 3 до 20 символів."; return; }

            if (string.IsNullOrEmpty(pass))
            { ErrorText.Text = "Введіть пароль."; return; }
            if (pass.Length < 4)
            { ErrorText.Text = "Пароль: мінімум 4 символи."; return; }
            if (pass != conf)
            { ErrorText.Text = "Паролі не збігаються."; return; }

            bool alreadyTaken = false;
            foreach (var acc in _authService.FetchAll())
                if (acc.Login.ToLower() == login.ToLower()) { alreadyTaken = true; break; }
            if (alreadyTaken) { ErrorText.Text = "Логін вже зайнятий."; return; }

            var newAccount = new UserAccount { Login = login, Password = pass, Role = "User" };
            _authService.Upsert(newAccount);
            LogService.Write(login, "зареєструвався");

            UserProfile.Login    = login;
            UserProfile.Role     = "User";
            UserProfile.Nickname = nick;
            new ProfileService().Persist();

            NavigationService.Navigate(new MenuPage());
        }
    }
}
