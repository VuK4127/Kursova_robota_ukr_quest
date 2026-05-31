namespace Kursova_robota_ukr_quest.Models
{
    public class GameResult
    {
        public string PlayerName  { get; set; } = "";
        public string Mode        { get; set; } = "";
        public int    Correct     { get; set; }
        public int    Total       { get; set; }
        public string Category    { get; set; } = "";
        public string Date        { get; set; } = "";
    }
}
