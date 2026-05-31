using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Kursova_robota_ukr_quest.Views
{
    // Represents one sentence split into parts with a gap slot
    public class PunctuationQuestion
    {
        public string Before { get; set; }   // text before the gap
        public string After  { get; set; }   // text after the gap
        public string Answer { get; set; }   // correct punctuation mark (empty = no sign)
        public string Hint   { get; set; }   // explanation shown after check
    }

    public partial class PunctuationPage : Page
    {
        private int _score = 0;
        private int _current = 0;
        private string _selected = null;        // sign chosen by user
        private Button _activeBtn = null;       // highlighted button

        private readonly List<PunctuationQuestion> _questions = new List<PunctuationQuestion>
        {
            // Koma — perelichennia
            new PunctuationQuestion
            {
                Before = "На столі лежали книги",
                After  = " зошити та ручки.",
                Answer = ",",
                Hint   = "Кома між однорідними членами речення."
            },
            // Tyre — pidmet = prykladka
            new PunctuationQuestion
            {
                Before = "Мова",
                After  = " душа народу.",
                Answer = "—",
                Hint   = "Тире між підметом і присудком, вираженими іменниками."
            },
            // Dvokrapka — poyasnennia
            new PunctuationQuestion
            {
                Before = "Я знаю одне",
                After  = " треба наполегливо працювати.",
                Answer = ":",
                Hint   = "Двокрапка, коли друга частина пояснює першу."
            },
            // Koma — zvertannia
            new PunctuationQuestion
            {
                Before = "Друже",
                After  = " розкажи мені про своє місто.",
                Answer = ",",
                Hint   = "Кома після звертання на початку речення."
            },
            // Tyre — BSR, naslidok
            new PunctuationQuestion
            {
                Before = "Посієш вчасно",
                After  = " збереш рясно.",
                Answer = "—",
                Hint   = "Тире в безсполучниковому реченні (умова — наслідок)."
            },
            // Koma — zkladne rechennia
            new PunctuationQuestion
            {
                Before = "Сонце зайшло за хмару",
                After  = " і стало прохолодно.",
                Answer = ",",
                Hint   = "Кома між частинами складносурядного речення."
            },
            // Vyklichnyi znak
            new PunctuationQuestion
            {
                Before = "Яка чудова українська природа",
                After  = "",
                Answer = "!",
                Hint   = "Знак оклику в окличному реченні."
            },
            // Dvokrapka — pryamamova
            new PunctuationQuestion
            {
                Before = "Мати сказала",
                After  = " «Будь обережний у дорозі».",
                Answer = ":",
                Hint   = "Двокрапка перед прямою мовою."
            },
            // Koma — vstavne slovo
            new PunctuationQuestion
            {
                Before = "Безперечно",
                After  = " Україна — прекрасна країна.",
                Answer = ",",
                Hint   = "Кома після вставного слова."
            },
            // Tyre — pidmet + prykladka (chyslo)
            new PunctuationQuestion
            {
                Before = "Двічі два",
                After  = " чотири.",
                Answer = "—",
                Hint   = "Тире між підметом і присудком без дієслова."
            },
            // Krapka z komoyu — BSR
            new PunctuationQuestion
            {
                Before = "Ліс шумів; птахи співали",
                After  = " природа прокидалась.",
                Answer = ";",
                Hint   = "Крапка з комою між відносно незалежними частинами."
            },
            // Pytalnyi znak
            new PunctuationQuestion
            {
                Before = "Ти вже виконав домашнє завдання",
                After  = "",
                Answer = "?",
                Hint   = "Знак питання в питальному реченні."
            },
            // Koma — oznachuvalne pidradne
            new PunctuationQuestion
            {
                Before = "Книга",
                After  = " яку я читав, була дуже цікавою.",
                Answer = ",",
                Hint   = "Кома перед підрядним означальним реченням."
            },
            // Tyre — BSR, prychyna-naslidok (Lesia Ukrainka style)
            new PunctuationQuestion
            {
                Before = "Виживе мова",
                After  = " виживе й Вітчизна.",
                Answer = "—",
                Hint   = "Тире в безсполучниковому реченні між рівнозначними частинами."
            },
            // Dvokrapka — uzahalnene slovo
            new PunctuationQuestion
            {
                Before = "Він любив усе рідне",
                After  = " мову, землю, пісню.",
                Answer = ":",
                Hint   = "Двокрапка після узагальнюючого слова перед переліком."
            },
        };

        public PunctuationPage()
        {
            InitializeComponent();
            RenderQuestion();
        }

        // ── Render ──────────────────────────────────────────────────────────────

        private void RenderQuestion()
        {
            _selected = null;
            _activeBtn = null;
            ResetPunctButtons();
            FeedbackBorder.Visibility = Visibility.Collapsed;
            CheckBtn.IsEnabled = false;
            NextBtn.IsEnabled = false;

            ProgressText.Text = $"{_current + 1} / {_questions.Count}";
            ScoreText.Text     = $"Бали: {_score}";

            var q = _questions[_current];
            SentencePanel.Items.Clear();

            if (!string.IsNullOrEmpty(q.Before))
                SentencePanel.Items.Add(MakeTextBlock(q.Before));

            SentencePanel.Items.Add(MakeGapButton());

            if (!string.IsNullOrEmpty(q.After))
                SentencePanel.Items.Add(MakeTextBlock(q.After));
        }

        private TextBlock MakeTextBlock(string text)
        {
            return new TextBlock
            {
                Text                = text,
                FontSize            = 18,
                FontFamily          = new FontFamily("Helvetica Neue, Arial, sans-serif"),
                Foreground          = (Brush)TryFindResource("MainTextBrush") ?? Brushes.Black,
                VerticalAlignment   = VerticalAlignment.Center,
                TextWrapping        = TextWrapping.Wrap,
                Margin              = new Thickness(0, 2, 0, 2)
            };
        }

        private Border MakeGapButton()
        {
            var tb = new TextBlock
            {
                Name              = "GapLabel",
                Text              = "  ___  ",
                FontSize          = 20,
                FontWeight        = FontWeights.Bold,
                Foreground        = Brushes.Gray,
                VerticalAlignment = VerticalAlignment.Center
            };

            var border = new Border
            {
                Name            = "GapBorder",
                MinWidth        = 44,
                MinHeight       = 36,
                CornerRadius    = new CornerRadius(6),
                BorderThickness = new Thickness(2),
                BorderBrush     = new SolidColorBrush(Color.FromRgb(26, 115, 232)),
                Background      = new SolidColorBrush(Color.FromArgb(20, 26, 115, 232)),
                Margin          = new Thickness(4, 2, 4, 2),
                Padding         = new Thickness(6, 2, 6, 2),
                Child           = tb
            };

            return border;
        }

        // Update the gap label with the chosen sign
        private void UpdateGapDisplay(string sign)
        {
            foreach (var item in SentencePanel.Items)
            {
                if (item is Border b && b.Name == "GapBorder" && b.Child is TextBlock tb)
                {
                    tb.Text       = string.IsNullOrEmpty(sign) ? "  ∅  " : $"  {sign}  ";
                    tb.Foreground = string.IsNullOrEmpty(sign)
                        ? Brushes.Gray
                        : new SolidColorBrush(Color.FromRgb(26, 115, 232));
                }
            }
        }

        // ── Punctuation button clicks ────────────────────────────────────────────

        private void PunctBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;

            // Deselect previous
            if (_activeBtn != null)
                _activeBtn.Background = new SolidColorBrush(Color.FromRgb(230, 230, 230));

            _activeBtn = btn;
            btn.Background = new SolidColorBrush(Color.FromRgb(26, 115, 232));
            // Change foreground to white when selected
            btn.Foreground = Brushes.White;

            _selected = btn.Tag?.ToString() ?? "";
            UpdateGapDisplay(_selected);
            CheckBtn.IsEnabled = true;
        }

        private void ResetPunctButtons()
        {
            var gray = new SolidColorBrush(Color.FromRgb(230, 230, 230));
            var dark = new SolidColorBrush(Color.FromRgb(40, 40, 40));
            foreach (var child in new[] { BtnComma, BtnPeriod, BtnDash, BtnColon,
                                          BtnExcl, BtnQuestion, BtnSemicolon, BtnNone })
            {
                child.Background = gray;
                child.Foreground = dark;
            }
        }

        // ── Check / Next ─────────────────────────────────────────────────────────

        private void CheckBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_selected == null) return;

            var q = _questions[_current];
            bool correct = _selected == q.Answer;

            FeedbackBorder.Visibility = Visibility.Visible;

            if (correct)
            {
                _score += 10;
                ScoreText.Text     = $"Бали: {_score}";
                StatusText.Text    = $"✔ Правильно! {q.Hint}";
                StatusText.Foreground = new SolidColorBrush(Color.FromRgb(52, 168, 83));
                FeedbackBorder.Background = new SolidColorBrush(Color.FromArgb(25, 52, 168, 83));
            }
            else
            {
                var correct_label = string.IsNullOrEmpty(q.Answer) ? "∅ (без знаку)" : $"«{q.Answer}»";
                StatusText.Text   = $"✘ Неправильно. Правильна відповідь: {correct_label}. {q.Hint}";
                StatusText.Foreground = new SolidColorBrush(Color.FromRgb(234, 67, 53));
                FeedbackBorder.Background = new SolidColorBrush(Color.FromArgb(25, 234, 67, 53));
            }

            CheckBtn.IsEnabled = false;
            NextBtn.IsEnabled  = true;
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            _current++;
            if (_current < _questions.Count)
            {
                RenderQuestion();
            }
            else
            {
                NavigationService.Navigate(new FinishPage("", _score));
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e) =>
            NavigationService.GoBack();
    }
}
