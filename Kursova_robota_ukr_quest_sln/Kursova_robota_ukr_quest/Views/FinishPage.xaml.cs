using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Kursova_robota_ukr_quest.Views
{
    public partial class FinishPage : Page
    {
        private int _level = 1;
        private GameMode _mode = GameMode.Grammar;

        public FinishPage() => InitializeComponent();

        public FinishPage(string name, int correct, int total, int level, GameMode mode) : this()
        {
            _level = level;
            _mode  = mode;

            ResultText.Text  = $"{correct} з {total}";
            int pct          = total > 0 ? (int)Math.Round(correct * 100.0 / total) : 0;
            PercentText.Text = $"{pct}%";

            // Emoji + comment based on score
            if (pct == 100)
            {
                EmojiText.Text    = "🏆";
                CommentText.Text  = "Чудово! Усі відповіді правильні!";
                ResultText.Foreground = new SolidColorBrush(Color.FromRgb(52, 168, 83));
            }
            else if (pct >= 80)
            {
                EmojiText.Text   = "🎉";
                CommentText.Text = "Відмінний результат! Так тримати!";
                ResultText.Foreground = new SolidColorBrush(Color.FromRgb(26, 115, 232));
            }
            else if (pct >= 60)
            {
                EmojiText.Text   = "👍";
                CommentText.Text = "Непогано! Є куди рости.";
                ResultText.Foreground = new SolidColorBrush(Color.FromRgb(251, 188, 5));
            }
            else
            {
                EmojiText.Text   = "💪";
                CommentText.Text = "Не здавайтесь — спробуйте ще раз!";
                ResultText.Foreground = new SolidColorBrush(Color.FromRgb(234, 67, 53));
            }
        }

        // Legacy constructor (used by PunctuationPage)
        public FinishPage(string name, int score) : this()
        {
            ResultText.Text  = $"{score} балів";
            PercentText.Text = "";
            EmojiText.Text   = "🎉";
            CommentText.Text = "Гру завершено!";
            RetryBtn.Visibility = Visibility.Collapsed;
        }

        private void Retry_Click(object sender, RoutedEventArgs e) =>
            NavigationService.Navigate(new GamePage(_level));

        private void LevelSelect_Click(object sender, RoutedEventArgs e) =>
            NavigationService.Navigate(new LevelSelectPage(_mode));

        private void ToMenu_Click(object sender, RoutedEventArgs e) =>
            NavigationService.Navigate(new MenuPage());
    }
}
