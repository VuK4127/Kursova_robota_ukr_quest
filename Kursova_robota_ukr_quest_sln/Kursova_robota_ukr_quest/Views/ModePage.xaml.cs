using System.Windows;
using System.Windows.Controls;

namespace Kursova_robota_ukr_quest.Views
{
    public partial class ModePage : Page
    {
        public ModePage()
        {
            InitializeComponent();
        }

        private void Classic_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new LevelSelectPage(GameMode.Grammar));
        }

        private void Wordle_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new WordlePage());
        }

        private void Punctuation_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PunctuationPage());
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MenuPage());
        }
    }
}