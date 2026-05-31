using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Kursova_robota_ukr_quest.Models;

namespace Kursova_robota_ukr_quest.Services
{
    public class DataService
    {
        private string BuildPath(string fileName) =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", fileName);

        public List<Question> ReadQuestions(string fileName = "grammar_questions.json")
        {
            string path = BuildPath(fileName);
            if (!File.Exists(path))
                throw new FileNotFoundException($"Файл питань не знайдено: {path}");
            string raw = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<Question>>(raw) ?? new List<Question>();
        }

        public void WriteQuestions(List<Question> items, string fileName = "grammar_questions.json")
        {
            string path = BuildPath(fileName);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                File.WriteAllText(path,
                    JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch (Exception ex)
            {
                throw new IOException($"Не вдалося записати питання: {ex.Message}", ex);
            }
        }

        public List<Question> ReadQuestionsByLevel(int level, string fileName = "grammar_questions.json")
        {
            var allItems = ReadQuestions(fileName);
            int startPos = (level - 1) * 10;
            int amount   = Math.Min(10, allItems.Count - startPos);
            if (amount <= 0) return new List<Question>();
            return allItems.GetRange(startPos, amount);
        }
    }
}
