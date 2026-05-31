using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Kursova_robota_ukr_quest.Views
{
    public enum GameMode { Grammar }

    public partial class LevelSelectPage : Page
    {
        private readonly GameMode _mode;
        private const int TotalLevels = 10;

        // Accent colours per mode
        private static readonly string GrammarColor = "#1A73E8";

        public LevelSelectPage(GameMode mode)
        {
            InitializeComponent();
            _mode = mode;
            TitleText.Text = mode == GameMode.Grammar
                ? "Граматика — рівень"
                : "Рівень";
            BuildLevelButtons();
        }

        private void BuildLevelButtons()
        {
            string accent = GrammarColor;

            for (int i = 1; i <= TotalLevels; i++)
            {
                int level = i; // capture for lambda

                var btn = new Button
                {
                    Content = $"Рівень {level}\n10 питань",
                    Style   = (Style)FindResource("MainButtonStyle"),
                    Background = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString(accent)),
                    Margin  = new Thickness(4, 4, 4, 4),
                    Height  = 54,
                    Tag     = level
                };

                // make the content wrap nicely
                btn.Content = BuildLevelContent(level);
                btn.Click  += (s, e) => StartLevel(level);

                LevelGrid.Children.Add(btn);
            }
        }

        private StackPanel BuildLevelContent(int level)
        {
            string accent = GrammarColor;
            var sp = new StackPanel { Orientation = Orientation.Horizontal };

            sp.Children.Add(new TextBlock
            {
                Text       = $"Рівень {level}",
                FontSize   = 14,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Center
            });

            sp.Children.Add(new TextBlock
            {
                Text       = "  · 10 питань",
                FontSize   = 12,
                Foreground = new SolidColorBrush(Color.FromArgb(200, 255, 255, 255)),
                VerticalAlignment = VerticalAlignment.Center
            });

            return sp;
        }

        private void StartLevel(int level)
        {
            NavigationService.Navigate(new GamePage(level));
        }

        private void Back_Click(object sender, RoutedEventArgs e) =>
            NavigationService.Navigate(new ModePage());
    }
}
