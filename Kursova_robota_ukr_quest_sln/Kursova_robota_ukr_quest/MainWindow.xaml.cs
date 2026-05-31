using System.Windows;

namespace Kursova_robota_ukr_quest
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new Views.LoginPage());
        }
    }
}
