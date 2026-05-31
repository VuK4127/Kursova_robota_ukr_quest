using System;
using System.IO;

namespace Kursova_robota_ukr_quest.Services
{
    // Дописує події з міткою часу у файл log.txt
    public static class LogService
    {
        private static readonly string _logPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");

        public static void Write(string actor, string eventText)
        {
            try
            {
                string entry = $"{DateTime.Now:HH:mm:ss} — {actor} {eventText}";
                File.AppendAllText(_logPath, entry + Environment.NewLine);
            }
            catch { /* не переривати роботу програми через збій журналу */ }
        }
    }
}
