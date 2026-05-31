using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Kursova_robota_ukr_quest.Models;

namespace Kursova_robota_ukr_quest.Services
{
    // Керує обліковими записами, що зберігаються у Data/users.json.
    // Dictionary<логін, UserAccount> забезпечує пошук за O(1).
    public class AuthService
    {
        private readonly string _filePath;
        // Структура даних #1 — Dictionary для швидкого пошуку облікового запису
        private Dictionary<string, UserAccount> _accountMap = new();

        public AuthService()
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "users.json");
            LoadFromDisk();
        }

        private void LoadFromDisk()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    _accountMap = new Dictionary<string, UserAccount>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["admin"] = new UserAccount { Login = "admin", Password = "admin123", Role = "Admin" },
                        ["user"]  = new UserAccount { Login = "user",  Password = "user123",  Role = "User"  }
                    };
                    WriteToDisk();
                    return;
                }
                var parsed = JsonSerializer.Deserialize<List<UserAccount>>(File.ReadAllText(_filePath))
                             ?? new List<UserAccount>();
                _accountMap = new Dictionary<string, UserAccount>(StringComparer.OrdinalIgnoreCase);
                foreach (var entry in parsed)
                    _accountMap[entry.Login] = entry;
            }
            catch { _accountMap = new Dictionary<string, UserAccount>(); }
        }

        public void WriteToDisk()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
            File.WriteAllText(_filePath,
                JsonSerializer.Serialize(new List<UserAccount>(_accountMap.Values),
                new JsonSerializerOptions { WriteIndented = true }));
        }

        // Повертає обліковий запис при збігу даних, інакше null
        public UserAccount? Authenticate(string login, string password)
        {
            if (_accountMap.TryGetValue(login, out var found) && found.Password == password)
                return found;
            return null;
        }

        public IEnumerable<UserAccount> FetchAll() => _accountMap.Values;

        public void Upsert(UserAccount acc) { _accountMap[acc.Login] = acc; WriteToDisk(); }
        public void Remove(string login)    { _accountMap.Remove(login);    WriteToDisk(); }
    }
}
