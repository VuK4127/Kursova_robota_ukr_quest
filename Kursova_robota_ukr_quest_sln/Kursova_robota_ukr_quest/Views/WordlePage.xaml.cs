using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Kursova_robota_ukr_quest.Models;

namespace Kursova_robota_ukr_quest.Views
{
    public class WordleCell : INotifyPropertyChanged
    {
        private string _letter = "";
        private Brush _cellBackground = Brushes.Transparent;
        private Brush _cellBorder = new SolidColorBrush(Color.FromRgb(200, 200, 200));
        private Brush _letterColor = new SolidColorBrush(Color.FromRgb(26, 26, 46));

        public string Letter
        {
            get => _letter;
            set { _letter = value; OnPropertyChanged(nameof(Letter)); }
        }
        public Brush CellBackground
        {
            get => _cellBackground;
            set { _cellBackground = value; OnPropertyChanged(nameof(CellBackground)); }
        }
        public Brush CellBorder
        {
            get => _cellBorder;
            set { _cellBorder = value; OnPropertyChanged(nameof(CellBorder)); }
        }
        public Brush LetterColor
        {
            get => _letterColor;
            set { _letterColor = value; OnPropertyChanged(nameof(LetterColor)); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public partial class WordlePage : Page
    {
        private const int MaxAttempts = 6;
        private const int WordLength = 5;

        private string _targetWord = "";
        private int _currentRow = 0;
        private bool _gameOver = false;
        private bool _suppressTextChanged = false; // guard flag

        private ObservableCollection<ObservableCollection<WordleCell>> _grid = new();

        private static readonly Brush GreenBg   = new SolidColorBrush(Color.FromRgb(52,  168, 83));
        private static readonly Brush YellowBg  = new SolidColorBrush(Color.FromRgb(251, 188, 5));
        private static readonly Brush GrayBg    = new SolidColorBrush(Color.FromRgb(120, 124, 126));
        private static readonly Brush WhiteFg   = Brushes.White;
        private static readonly Brush DarkFg    = new SolidColorBrush(Color.FromRgb(26,  26,  46));
        private static readonly Brush BorderActive = new SolidColorBrush(Color.FromRgb(26, 115, 232));
        private static readonly Brush BorderEmpty  = new SolidColorBrush(Color.FromRgb(200, 200, 200));

        public WordlePage() => InitializeComponent();

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            InitGame();
            GuessInput.Focus();
        }

        private void InitGame()
        {
            _targetWord = WordleWordList.GetRandomWord();
            _currentRow = 0;
            _gameOver = false;

            StatusText.Text = "";

            _suppressTextChanged = true;
            GuessInput.Text = "";
            _suppressTextChanged = false;

            GuessInput.IsEnabled = true;

            // Hide game-over overlay
            GameOverOverlay.Visibility = Visibility.Collapsed;

            _grid = new ObservableCollection<ObservableCollection<WordleCell>>();
            for (int r = 0; r < MaxAttempts; r++)
            {
                var row = new ObservableCollection<WordleCell>();
                for (int c = 0; c < WordLength; c++)
                    row.Add(new WordleCell());
                _grid.Add(row);
            }
            GuessGrid.ItemsSource = _grid;
        }

        private void GuessInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_suppressTextChanged) return;
            if (_gameOver) return;
            if (_currentRow >= MaxAttempts) return; // safety guard

            var text = GuessInput.Text.ToUpper();
            var row = _grid[_currentRow];

            for (int i = 0; i < WordLength; i++)
            {
                if (i < text.Length)
                {
                    row[i].Letter = text[i].ToString();
                    row[i].CellBorder = BorderActive;
                }
                else
                {
                    row[i].Letter = "";
                    row[i].CellBorder = BorderEmpty;
                }
                row[i].CellBackground = Brushes.Transparent;
                row[i].LetterColor = DarkFg;
            }
        }

        private void GuessInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) ProcessGuess();
        }

        private void Submit_Click(object sender, RoutedEventArgs e) => ProcessGuess();

        private void ProcessGuess()
        {
            if (_gameOver) return;

            var guess = GuessInput.Text.Trim().ToUpper();

            if (guess.Length != WordLength)
            {
                StatusText.Text = $"Введіть рівно {WordLength} букв!";
                StatusText.Foreground = new SolidColorBrush(Color.FromRgb(220, 53, 69));
                return;
            }

            if (!WordleWordList.IsValidWord(guess))
            {
                StatusText.Text = "Слово не знайдено у словнику!";
                StatusText.Foreground = new SolidColorBrush(Color.FromRgb(220, 53, 69));
                return;
            }

            // Score and colour the row
            var result = EvaluateGuess(guess, _targetWord);
            var row = _grid[_currentRow];

            for (int i = 0; i < WordLength; i++)
            {
                row[i].Letter = guess[i].ToString();
                row[i].CellBorder = Brushes.Transparent;
                row[i].LetterColor = WhiteFg;
                row[i].CellBackground = result[i] switch
                {
                    2 => GreenBg,
                    1 => YellowBg,
                    _ => GrayBg
                };
            }

            // Increment row BEFORE clearing input (prevents TextChanged crash)
            _currentRow++;

            // Clear input safely
            _suppressTextChanged = true;
            GuessInput.Text = "";
            _suppressTextChanged = false;

            if (guess == _targetWord)
            {
                _gameOver = true;
                GuessInput.IsEnabled = false;
                StatusText.Text = "🎉 Вітаємо! Ви вгадали слово!";
                StatusText.Foreground = GreenBg;
            }
            else if (_currentRow >= MaxAttempts)
            {
                _gameOver = true;
                GuessInput.IsEnabled = false;
                DisplayGameOver();
            }
            else
            {
                StatusText.Text = "";
                GuessInput.Focus();
            }
        }

        private void DisplayGameOver()
        {
            GameOverWord.Text = _targetWord;
            GameOverOverlay.Visibility = Visibility.Visible;
        }

        private void OverlayRestart_Click(object sender, RoutedEventArgs e)
        {
            InitGame();
            GuessInput.Focus();
        }

        private void OverlayMenu_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MenuPage());
        }

        private int[] EvaluateGuess(string guess, string target)
        {
            var result = new int[WordLength];
            var targetCounts = new Dictionary<char, int>();

            foreach (char c in target)
            {
                if (!targetCounts.ContainsKey(c)) targetCounts[c] = 0;
                targetCounts[c]++;
            }

            // Pass 1: greens
            for (int i = 0; i < WordLength; i++)
            {
                if (guess[i] == target[i])
                {
                    result[i] = 2;
                    targetCounts[guess[i]]--;
                }
            }

            // Pass 2: yellows
            for (int i = 0; i < WordLength; i++)
            {
                if (result[i] == 2) continue;
                if (targetCounts.TryGetValue(guess[i], out int count) && count > 0)
                {
                    result[i] = 1;
                    targetCounts[guess[i]]--;
                }
            }

            return result;
        }

        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            InitGame();
            GuessInput.Focus();
        }

        private void Back_Click(object sender, RoutedEventArgs e) =>
            NavigationService.GoBack();
    }
}
