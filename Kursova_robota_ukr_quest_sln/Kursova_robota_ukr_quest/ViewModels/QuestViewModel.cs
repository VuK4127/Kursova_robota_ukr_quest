using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Kursova_robota_ukr_quest.Models;
using Kursova_robota_ukr_quest.Services;

namespace Kursova_robota_ukr_quest.ViewModels
{
    public class QuestViewModel : INotifyPropertyChanged
    {
        private readonly DataService _dataService = new();
        private ObservableCollection<Question> _questions = null!;
        private User _activeUser = null!;
        private string _currentText = string.Empty;
        private int _idx = 0;

        public int Level        { get; private set; } = 1;
        public int CorrectCount { get; private set; } = 0;
        public int TotalCount   => Questions?.Count ?? 0;

        public QuestViewModel() : this(1, "grammar_questions.json") { }

        public QuestViewModel(int level, string fileName = "grammar_questions.json")
        {
            Level      = level;
            CurrentUser = new User { Name = UserProfile.Nickname, Score = 0 };
            FetchQuestions(level, fileName);
        }

        private void FetchQuestions(int level, string fileName)
        {
            try
            {
                var data = _dataService.ReadQuestionsByLevel(level, fileName);
                Questions = new ObservableCollection<Question>(data);
                SyncText();
            }
            catch
            {
                Questions     = new ObservableCollection<Question>();
                CurrentText   = "Помилка: файл питань не знайдено!";
            }
        }

        public User   CurrentUser { get => _activeUser;  set { _activeUser  = value; OnPropertyChanged(); } }
        public ObservableCollection<Question> Questions { get => _questions; set { _questions = value; OnPropertyChanged(); } }
        public string CurrentText { get => _currentText; set { _currentText = value; OnPropertyChanged(); } }

        public Question? CurrentQuestion =>
            (Questions != null && _idx < Questions.Count) ? Questions[_idx] : null;

        public void SyncText()
        {
            if (CurrentQuestion != null) CurrentText = CurrentQuestion.Text;
        }

        public void RegisterCorrect() => CorrectCount++;

        public bool NextQuestion()
        {
            if (_idx < Questions.Count - 1)
            {
                _idx++;
                SyncText();
                return true;
            }
            return false;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
