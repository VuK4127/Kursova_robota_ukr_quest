namespace Kursova_robota_ukr_quest.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string CorrectAnswer { get; set; }
        public string Category { get; set; }
        public int Difficulty { get; set; }
    }
}