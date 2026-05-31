namespace Kursova_robota_ukr_quest.Models
{
    public class UserAccount
    {
        public string Login    { get; set; } = "";
        public string Password { get; set; } = "";   // plain-text for course-work scope
        public string Role     { get; set; } = "User"; // "Admin" or "User"
    }
}
