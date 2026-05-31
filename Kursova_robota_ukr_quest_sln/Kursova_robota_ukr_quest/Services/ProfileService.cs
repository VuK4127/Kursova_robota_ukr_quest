using System;
using System.IO;
using System.Text.Json;
using Kursova_robota_ukr_quest.Models;

namespace Kursova_robota_ukr_quest.Services
{
    // Зберігає та відновлює UserProfile у Data/profile.json
    public class ProfileService
    {
        private readonly string _storagePath;

        public ProfileService()
        {
            _storagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "profile.json");
        }

        public void Persist()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_storagePath)!);
                var snapshot = new
                {
                    UserProfile.Nickname,
                    UserProfile.AvatarPath,
                    UserProfile.Login,
                    UserProfile.Role
                };
                File.WriteAllText(_storagePath,
                    JsonSerializer.Serialize(snapshot, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch { }
        }

        public void Restore()
        {
            try
            {
                if (!File.Exists(_storagePath)) return;
                using var doc  = JsonDocument.Parse(File.ReadAllText(_storagePath));
                var root = doc.RootElement;
                if (root.TryGetProperty("Nickname",   out var n)) UserProfile.Nickname   = n.GetString() ?? "Гравець";
                if (root.TryGetProperty("AvatarPath", out var a)) UserProfile.AvatarPath = a.GetString() ?? "";
                if (root.TryGetProperty("Login",      out var l)) UserProfile.Login      = l.GetString() ?? "";
                if (root.TryGetProperty("Role",       out var r)) UserProfile.Role       = r.GetString() ?? "User";
            }
            catch { }
        }
    }
}
