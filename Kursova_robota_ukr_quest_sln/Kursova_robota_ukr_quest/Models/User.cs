using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Kursova_robota_ukr_quest.Models
{
    public class User : INotifyPropertyChanged
    {
        private string _name  = "";
        private int    _score = 0;

        public string Name
        {
            get => _name;
            set { _name = value; Notify(); }
        }

        public int Score
        {
            get => _score;
            set { _score = value; Notify(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void Notify([CallerMemberName] string? prop = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
