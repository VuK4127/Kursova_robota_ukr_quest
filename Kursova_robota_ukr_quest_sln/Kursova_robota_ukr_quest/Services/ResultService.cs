using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Kursova_robota_ukr_quest.Models;

namespace Kursova_robota_ukr_quest.Services
{
    // Зберігає та завантажує історію ігор. Містить власні алгоритми сортування та пошуку.
    public class ResultService
    {
        private readonly string _storagePath;

        // Структура даних #2 — List для зберігання хронологічної історії
        private List<GameResult> _history = new();

        // Структура даних #3 — HashSet унікальних пройдених категорій
        public HashSet<string> UniqueCategories { get; } = new(StringComparer.OrdinalIgnoreCase);

        // Структура даних #4 — Stack останніх переглянутих записів
        private Stack<int> _browseStack = new();

        public ResultService()
        {
            _storagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "results.json");
            LoadFromDisk();
        }

        // ── Серіалізація ────────────────────────────────────────────────────────

        public void LoadFromDisk()
        {
            try
            {
                if (!File.Exists(_storagePath)) return;
                _history = JsonSerializer.Deserialize<List<GameResult>>(File.ReadAllText(_storagePath))
                           ?? new List<GameResult>();
                foreach (var item in _history)
                    UniqueCategories.Add(item.Category);
            }
            catch { _history = new List<GameResult>(); }
        }

        public void SaveToDisk()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_storagePath)!);
                File.WriteAllText(_storagePath,
                    JsonSerializer.Serialize(_history, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch { }
        }

        // ── CRUD ────────────────────────────────────────────────────────────────

        public void Append(GameResult r)
        {
            _history.Add(r);
            UniqueCategories.Add(r.Category);
            SaveToDisk();
        }

        public List<GameResult> GetAll() => new List<GameResult>(_history);

        // ── QuickSort — сортування за двома полями: Category ASC, Correct DESC ──

        public List<GameResult> GetSorted()
        {
            var copy = new List<GameResult>(_history);
            RunQuickSort(copy, 0, copy.Count - 1);
            return copy;
        }

        private static void RunQuickSort(List<GameResult> arr, int left, int right)
        {
            if (left >= right) return;
            int pivotIdx = SplitPartition(arr, left, right);
            RunQuickSort(arr, left, pivotIdx - 1);
            RunQuickSort(arr, pivotIdx + 1, right);
        }

        private static int SplitPartition(List<GameResult> arr, int left, int right)
        {
            var pivotElem = arr[right];
            int wall = left - 1;
            for (int k = left; k < right; k++)
            {
                if (CompareResults(arr[k], pivotElem) <= 0)
                {
                    wall++;
                    (arr[wall], arr[k]) = (arr[k], arr[wall]);
                }
            }
            (arr[wall + 1], arr[right]) = (arr[right], arr[wall + 1]);
            return wall + 1;
        }

        private static int CompareResults(GameResult x, GameResult y)
        {
            int categoryOrder = string.Compare(x.Category, y.Category, StringComparison.OrdinalIgnoreCase);
            if (categoryOrder != 0) return categoryOrder;
            return y.Correct.CompareTo(x.Correct); // більший бал — вище
        }

        // ── Бінарний пошук за іменем гравця (список має бути відсортованим) ────

        public int BinaryFindByName(List<GameResult> sortedList, string targetName)
        {
            int lo = 0, hi = sortedList.Count - 1;
            while (lo <= hi)
            {
                int mid = (lo + hi) / 2;
                int cmp = string.Compare(sortedList[mid].PlayerName, targetName, StringComparison.OrdinalIgnoreCase);
                if (cmp == 0) return mid;
                if (cmp < 0)  lo = mid + 1;
                else          hi = mid - 1;
            }
            return -1;
        }

        // ── Нечіткий пошук Левенштейна ──────────────────────────────────────────

        public static int LevenshteinDistance(string src, string dst)
        {
            src = src.ToLower();
            dst = dst.ToLower();
            int[,] dp = new int[src.Length + 1, dst.Length + 1];
            for (int i = 0; i <= src.Length; i++) dp[i, 0] = i;
            for (int j = 0; j <= dst.Length; j++) dp[0, j] = j;
            for (int i = 1; i <= src.Length; i++)
                for (int j = 1; j <= dst.Length; j++)
                    dp[i, j] = src[i - 1] == dst[j - 1]
                        ? dp[i - 1, j - 1]
                        : 1 + Math.Min(dp[i - 1, j - 1], Math.Min(dp[i - 1, j], dp[i, j - 1]));
            return dp[src.Length, dst.Length];
        }

        // Повертає записи, де ім'я гравця відрізняється не більше ніж на threshold правок
        public List<GameResult> FuzzySearchByName(string query, int threshold = 3)
        {
            var matches = new List<GameResult>();
            foreach (var r in _history)
                if (LevenshteinDistance(query, r.PlayerName) <= threshold)
                    matches.Add(r);
            return matches;
        }

        // ── Стек перегляду ───────────────────────────────────────────────────────

        public void TrackView(int index)
        {
            _browseStack.Push(index);
            if (_browseStack.Count > 10)
            {
                var temp = new Stack<int>();
                int kept = 0;
                foreach (var v in _browseStack) { if (kept++ < 10) temp.Push(v); }
                _browseStack = new Stack<int>(temp);
            }
        }

        public int? PopLastView() => _browseStack.Count > 0 ? _browseStack.Pop() : null;

        // ── Бізнес-логіка: аналіз слабких тем ──────────────────────────────────

        // Агрегує точність по кожній категорії.
        // Категорії з точністю нижче 60% позначаються для повторення.
        public List<(string Category, int Played, double Accuracy)> AnalyseWeakTopics()
        {
            var aggregated = new Dictionary<string, (int correct, int total)>(StringComparer.OrdinalIgnoreCase);
            foreach (var r in _history)
            {
                if (!aggregated.ContainsKey(r.Category))
                    aggregated[r.Category] = (0, 0);
                var (c, t) = aggregated[r.Category];
                aggregated[r.Category] = (c + r.Correct, t + r.Total);
            }

            var output = new List<(string, int, double)>();
            foreach (var kv in aggregated)
            {
                double pct = kv.Value.total > 0
                    ? Math.Round(kv.Value.correct * 100.0 / kv.Value.total, 1)
                    : 0;
                output.Add((kv.Key, kv.Value.total, pct));
            }
            output.Sort((a, b) => a.Item3.CompareTo(b.Item3)); // найгірше — перше
            return output;
        }
    }
}
