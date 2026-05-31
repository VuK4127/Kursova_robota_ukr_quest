using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Kursova_robota_ukr_quest.Models;
using Kursova_robota_ukr_quest.Services;
using Kursova_robota_ukr_quest.ViewModels;

namespace Kursova_robota_ukr_quest.Views
{
    public partial class GamePage : Page
    {
        private QuestViewModel _vm;
        private int _questionIndex = 0;
        private int _chosenLevel   = 1;
        private readonly ResultService _resultService = new();

        public GamePage() : this(1) { }

        public GamePage(int level)
        {
            InitializeComponent();
            _chosenLevel = level;
            _vm = new QuestViewModel(level, "grammar_questions.json");
            this.DataContext = _vm;
            LevelLabel.Text = $"Рівень {level}";
            RefreshProgress();
            LogService.Write(UserProfile.Login, $"почав гру Граматика рівень {level}");
        }

        private void RefreshProgress()
        {
            ProgressText.Text = $" · {_questionIndex + 1}/{_vm.TotalCount}";
            CorrectText.Text  = $"✔ {_vm.CorrectCount}";
        }

        private void AnswerInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (NextBtn.IsEnabled) GoNext();
                else CheckAnswer();
            }
        }

        private void CheckBtn_Click(object sender, RoutedEventArgs e) => CheckAnswer();

        private void CheckAnswer()
        {
            if (_vm.CurrentQuestion == null) return;

            if (string.IsNullOrWhiteSpace(AnswerInput.Text))
            {
                StatusText.Text       = "⚠ Введіть відповідь!";
                StatusText.Foreground = System.Windows.Media.Brushes.Orange;
                return;
            }

            bool isCorrect = AnswerInput.Text.Trim().ToLower() ==
                             _vm.CurrentQuestion.CorrectAnswer.ToLower();

            if (isCorrect)
            {
                _vm.RegisterCorrect();
                CorrectText.Text      = $"✔ {_vm.CorrectCount}";
                StatusText.Text       = "✔ Правильно!";
                StatusText.Foreground = System.Windows.Media.Brushes.Green;
            }
            else
            {
                StatusText.Text       = $"✘ Неправильно. Правильна відповідь: {_vm.CurrentQuestion.CorrectAnswer}";
                StatusText.Foreground = System.Windows.Media.Brushes.Red;
            }

            CheckBtn.IsEnabled = false;
            NextBtn.IsEnabled  = true;
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e) => GoNext();

        private void GoNext()
        {
            if (_vm.NextQuestion())
            {
                _questionIndex++;
                RefreshProgress();
                AnswerInput.Clear();
                AnswerInput.Focus();
                StatusText.Text    = "";
                CheckBtn.IsEnabled = true;
                NextBtn.IsEnabled  = false;
            }
            else
            {
                var record = new GameResult
                {
                    PlayerName = UserProfile.Nickname,
                    Mode       = "Граматика",
                    Correct    = _vm.CorrectCount,
                    Total      = _vm.TotalCount,
                    Category   = "Граматика",
                    Date       = DateTime.Now.ToString("dd.MM.yyyy HH:mm")
                };
                _resultService.Append(record);
                LogService.Write(UserProfile.Login,
                    $"завершив Граматика рівень {_chosenLevel}: {_vm.CorrectCount}/{_vm.TotalCount}");

                NavigationService.Navigate(
                    new FinishPage(_vm.CurrentUser.Name,
                                   _vm.CorrectCount,
                                   _vm.TotalCount,
                                   _chosenLevel,
                                   GameMode.Grammar));
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e) =>
            NavigationService.Navigate(new LevelSelectPage(GameMode.Grammar));
    }
}
