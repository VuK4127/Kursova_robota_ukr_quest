using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Kursova_robota_ukr_quest.Models;

namespace Kursova_robota_ukr_quest.Services
{
    // Формує текстовий аналітичний звіт та зберігає його у report.txt
    public class ReportService
    {
        private readonly ResultService _results;

        public ReportService(ResultService rs) => _results = rs;

        public string Build()
        {
            var sb = new StringBuilder();
            sb.AppendLine("================================================");
            sb.AppendLine("       МОВНИЙ КВЕСТ — АНАЛІТИЧНИЙ ЗВІТ");
            sb.AppendLine($"       Сформовано: {DateTime.Now:dd.MM.yyyy  HH:mm}");
            sb.AppendLine("================================================");
            sb.AppendLine();

            var sorted = _results.GetSorted();
            sb.AppendLine($"Всього ігор: {sorted.Count}");
            sb.AppendLine($"Пройдені категорії: {string.Join(", ", _results.UniqueCategories)}");
            sb.AppendLine();

            sb.AppendLine("── Результати (за категорією / балами) ──");
            foreach (var r in sorted)
            {
                int pct = r.Total > 0 ? (int)Math.Round(r.Correct * 100.0 / r.Total) : 0;
                sb.AppendLine($"  {r.Date,-20} {r.PlayerName,-15} [{r.Mode,-12}] {r.Category,-15} {r.Correct}/{r.Total} ({pct}%)");
            }

            sb.AppendLine();
            sb.AppendLine("── Слабкі теми ──");
            var weakTopics = _results.AnalyseWeakTopics();
            if (weakTopics.Count == 0)
            {
                sb.AppendLine("  Недостатньо даних для аналізу.");
            }
            else
            {
                foreach (var (cat, played, acc) in weakTopics)
                {
                    string flag = acc < 60 ? " ⚠ Рекомендовано повторити!" : "";
                    sb.AppendLine($"  {cat,-20} точність: {acc,5:F1}%  (питань: {played}){flag}");
                }
            }

            sb.AppendLine();
            sb.AppendLine("================================================");
            return sb.ToString();
        }

        public string SaveReport()
        {
            string text = Build();
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "report.txt");
            try { File.WriteAllText(path, text, Encoding.UTF8); }
            catch { }
            return path;
        }
    }
}
