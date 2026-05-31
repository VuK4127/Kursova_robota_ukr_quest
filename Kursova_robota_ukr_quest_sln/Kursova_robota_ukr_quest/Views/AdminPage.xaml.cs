using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Kursova_robota_ukr_quest.Models;
using Kursova_robota_ukr_quest.Services;

namespace Kursova_robota_ukr_quest.Views
{
    public partial class AdminPage : Page
    {
        private readonly DataService   _dataService = new();
        private readonly ResultService _resultService = new();
        private readonly ReportService _reportService;

        private List<Question> _questionBank = new();
        private Question? _chosenQuestion;

        public AdminPage()
        {
            InitializeComponent();
            _reportService = new ReportService(_resultService);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e) => RefreshGrid();

        private void RefreshGrid()
        {
            try
            {
                _questionBank = _dataService.ReadQuestions("grammar_questions.json");
                QuestionsGrid.ItemsSource = new List<Question>(_questionBank);
                StatusText.Text = $"Завантажено {_questionBank.Count} питань.";
                LogService.Write(UserProfile.Login, "відкрив адмін-панель");
            }
            catch (Exception ex) { StatusText.Text = "Помилка: " + ex.Message; }
        }

        // ── Пошук ────────────────────────────────────────────────────────────────

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string term = SearchBox.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(term)) { RefreshGrid(); return; }

            // 1. Точний збіг підрядком
            var hits = _questionBank.Where(x =>
                x.Text.ToLower().Contains(term) ||
                x.CorrectAnswer.ToLower().Contains(term) ||
                x.Category.ToLower().Contains(term)).ToList();

            // 2. Нечіткий пошук Левенштейна, якщо точних немає
            if (hits.Count == 0)
            {
                hits = _questionBank.Where(x =>
                    ResultService.LevenshteinDistance(term, x.Category.ToLower()) <= 3 ||
                    ResultService.LevenshteinDistance(term, x.CorrectAnswer.ToLower()) <= 3).ToList();
                StatusText.Text = hits.Count > 0
                    ? $"Точних результатів не знайдено. Нечіткий пошук: {hits.Count} збігів."
                    : "Нічого не знайдено.";
            }
            else
            {
                StatusText.Text = $"Знайдено: {hits.Count} питань.";
            }

            QuestionsGrid.ItemsSource = hits;
        }

        private void ShowAll_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "";
            RefreshGrid();
        }

        // ── Вибір рядка ──────────────────────────────────────────────────────────

        private void QuestionsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _chosenQuestion = QuestionsGrid.SelectedItem as Question;
            if (_chosenQuestion == null) return;

            TxtQuestion.Text   = _chosenQuestion.Text;
            TxtAnswer.Text     = _chosenQuestion.CorrectAnswer;
            TxtCategory.Text   = _chosenQuestion.Category;
            TxtDifficulty.Text = _chosenQuestion.Difficulty.ToString();
            DeleteBtn.IsEnabled = true;
        }

        // ── CRUD ─────────────────────────────────────────────────────────────────

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields(out string err)) { StatusText.Text = err; return; }

            var q = new Question
            {
                Id            = _questionBank.Count > 0 ? _questionBank.Max(x => x.Id) + 1 : 1,
                Text          = TxtQuestion.Text.Trim(),
                CorrectAnswer = TxtAnswer.Text.Trim(),
                Category      = TxtCategory.Text.Trim(),
                Difficulty    = int.Parse(TxtDifficulty.Text.Trim())
            };
            _questionBank.Add(q);
            _dataService.WriteQuestions(_questionBank, "grammar_questions.json");
            LogService.Write(UserProfile.Login, $"додав питання ID {q.Id}");
            StatusText.Text = $"Питання #{q.Id} додано.";
            RefreshGrid();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (_chosenQuestion == null) { StatusText.Text = "Оберіть питання для редагування."; return; }
            if (!ValidateFields(out string err)) { StatusText.Text = err; return; }

            _chosenQuestion.Text          = TxtQuestion.Text.Trim();
            _chosenQuestion.CorrectAnswer = TxtAnswer.Text.Trim();
            _chosenQuestion.Category      = TxtCategory.Text.Trim();
            _chosenQuestion.Difficulty    = int.Parse(TxtDifficulty.Text.Trim());
            _dataService.WriteQuestions(_questionBank, "grammar_questions.json");
            LogService.Write(UserProfile.Login, $"редагував питання ID {_chosenQuestion.Id}");
            StatusText.Text = $"Питання #{_chosenQuestion.Id} оновлено.";
            RefreshGrid();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (_chosenQuestion == null) return;
            var confirm = MessageBox.Show($"Видалити питання #{_chosenQuestion.Id}?",
                "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm != MessageBoxResult.Yes) return;
            LogService.Write(UserProfile.Login, $"видалив питання ID {_chosenQuestion.Id}");
            _questionBank.Remove(_chosenQuestion);
            _dataService.WriteQuestions(_questionBank, "grammar_questions.json");
            StatusText.Text = "Питання видалено.";
            DeleteBtn.IsEnabled = false;
            _chosenQuestion = null;
            RefreshGrid();
        }

        private void Report_Click(object sender, RoutedEventArgs e)
        {
            string path = _reportService.SaveReport();
            LogService.Write(UserProfile.Login, "згенерував звіт report.txt");
            StatusText.Text = $"Звіт збережено: {path}";
            MessageBox.Show($"Звіт збережено:\n{path}", "Звіт", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Back_Click(object sender, RoutedEventArgs e) =>
            NavigationService.Navigate(new MenuPage());

        // ── Валідація ─────────────────────────────────────────────────────────────

        private bool ValidateFields(out string error)
        {
            error = "";
            if (string.IsNullOrWhiteSpace(TxtQuestion.Text))
            { error = "Введіть текст питання."; return false; }
            if (TxtQuestion.Text.Trim().Length < 5)
            { error = "Текст питання занадто короткий (мінімум 5 символів)."; return false; }
            if (string.IsNullOrWhiteSpace(TxtAnswer.Text))
            { error = "Введіть правильну відповідь."; return false; }
            if (string.IsNullOrWhiteSpace(TxtCategory.Text))
            { error = "Введіть категорію."; return false; }
            if (!int.TryParse(TxtDifficulty.Text.Trim(), out int diff) || diff < 1 || diff > 10)
            { error = "Рівень складності — ціле число від 1 до 10."; return false; }
            return true;
        }
    }
}
