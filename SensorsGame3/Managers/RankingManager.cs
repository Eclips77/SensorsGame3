using System;
using System.Collections.Generic;
using System.Linq;

namespace SensorsGame3.Managers
{
    public static class RankingManager
    {
        public static void DisplayRankings()
        {
            var history = GameHistory.Instance;
            var entries = history.GetAllEntries();
            var scoresByPlayer = new Dictionary<string, List<int>>();
            foreach (var entry in entries)
            {
                if (!scoresByPlayer.ContainsKey(entry.Username))
                    scoresByPlayer[entry.Username] = new List<int>();
                scoresByPlayer[entry.Username].Add(entry.Score);
            }
            //return players list order by score
            var rankings = scoresByPlayer
                .Select(kvp => new { Player = kvp.Key, Score = kvp.Value.Sum() })
                .OrderByDescending(x => x.Score)
                .ToList();

            Console.WriteLine("=== PLAYER RANKINGS ===");
            int pos = 1;
            foreach (var r in rankings)
            {
                Console.WriteLine($"{pos}. {r.Player} - {r.Score} pts");
                pos++;
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
