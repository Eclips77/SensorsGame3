using System;
using System.Collections.Generic;
using System.Linq;
using SensorsGame3.Entities;
using SensorsGame3.Tools;

namespace SensorsGame3.Managers
{
    public class GameHistory
    {
        private static GameHistory _instance;
        private readonly HistoryDal _historyDal;

        private GameHistory()
        {
            var conn = Environment.GetEnvironmentVariable("GAME_DB_CONN") ??
                        "server=localhost;user id=root;password=;database=game";
            _historyDal = new HistoryDal(conn);
        }

        public static GameHistory Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameHistory();
                }
                return _instance;
            }
        }

        public void AddSession(GameSession session)
        {
            try
            {
                var entry = new GameHistoryEntry
                {
                    Username = session.Username,
                    AgentType = session.AgentRank.ToString(),
                    WeaknessCombination = session.WeaknessCombination,
                    UsedSensors = session.UsedSensors,
                    CorrectSensors = session.CorrectSensors,
                    TurnsTaken = session.Attempts,
                    Score = session.Score,
                    Victory = session.Victory,
                    Timestamp = session.Date
                };
                _historyDal.Insert(entry);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GameHistory.AddSession: {ex.Message}");
            }
        }

        public void DisplayHistory(string username = null)
        {
            Console.Clear();
            Console.WriteLine("=== GAME HISTORY ===");

            List<GameHistoryEntry> entries = new List<GameHistoryEntry>();
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    entries = _historyDal.GetAll();
                }
                else
                {
                    entries = _historyDal.GetByUser(username);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GameHistory.DisplayHistory: {ex.Message}");
                return;
            }
            int gamesPlayed = entries.Count;
            int totalScore = 0;
            int totalTurns = 0;
            int victories = 0;
            var agentsExposed = new Dictionary<AgentRank, int>();
            var sensorsUsed = new Dictionary<SensorType, int>();

            foreach (var e in entries)
            {
                if (Enum.TryParse(e.AgentType, out AgentRank rank))
                {
                    if (!agentsExposed.ContainsKey(rank))
                        agentsExposed[rank] = 0;
                    if (e.Victory)
                    {
                        agentsExposed[rank]++;
                        victories++;
                    }

                    totalScore += e.Score;
                    totalTurns += e.TurnsTaken;
                }

                foreach (var s in e.UsedSensors)
                {
                    if (!sensorsUsed.ContainsKey(s))
                        sensorsUsed[s] = 0;
                    sensorsUsed[s]++;
                }
            }

            Console.WriteLine($"Total Games Played: {gamesPlayed}");
            Console.WriteLine($"Total Score: {totalScore}");
            Console.WriteLine($"Average Score: {(gamesPlayed > 0 ? totalScore / gamesPlayed : 0)}");
            Console.WriteLine($"Success Rate: {(gamesPlayed > 0 ? (double)victories / gamesPlayed * 100 : 0):F2}%");
            Console.WriteLine($"Average Turns: {(gamesPlayed > 0 ? totalTurns / gamesPlayed : 0)}");
            string title = "Rookie";
            int avgScore = gamesPlayed > 0 ? totalScore / gamesPlayed : 0;
            if (avgScore > 150) title = "Master Investigator";
            else if (avgScore > 80) title = "Agent Hunter";
            Console.WriteLine($"Title: {title}");

            Console.WriteLine("\nAgents Exposed:");
            foreach (var agent in agentsExposed)
            {
                Console.WriteLine($"- {agent.Key}: {agent.Value} times");
            }

            Console.WriteLine("\nSensors Used:");
            foreach (var sensor in sensorsUsed)
            {
                Console.WriteLine($"- {sensor.Key}: {sensor.Value} times");
            }

            Console.WriteLine("\nRecent Sessions:");
            foreach (var entry in entries.OrderByDescending(e => e.Timestamp).Take(5))
            {
                Console.WriteLine($"\nDate: {entry.Timestamp}");
                Console.WriteLine($"Agent: {entry.AgentType}");
                Console.WriteLine($"Turns: {entry.TurnsTaken}");
                Console.WriteLine($"Victory: {entry.Victory}");
                Console.WriteLine($"Sensors Used: {string.Join(", ", entry.UsedSensors)}");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        public List<GameHistoryEntry> GetAllEntries()
        {
            try
            {
                return _historyDal.GetAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GameHistory.GetAllEntries: {ex.Message}");
                return new List<GameHistoryEntry>();
            }
        }
    }

    public class GameSession
    {
        public DateTime Date { get; }
        public AgentRank AgentRank { get; }
        public int Score { get; }
        public string Username { get; }
        public List<SensorType> UsedSensors { get; }
        public int Attempts { get; }
        public bool Victory { get; }
        public List<SensorType> WeaknessCombination { get; }
        public List<SensorType> CorrectSensors { get; }

        public GameSession(string username, AgentRank agentRank, int score, List<SensorType> usedSensors, int attempts, bool victory, List<SensorType> weaknesses)
        {
            Date = DateTime.Now;
            AgentRank = agentRank;
            Score = score;
            Username = username;
            UsedSensors = new List<SensorType>(usedSensors);
            Attempts = attempts;
            Victory = victory;
            WeaknessCombination = new List<SensorType>(weaknesses);
            CorrectSensors = UsedSensors.Intersect(weaknesses).ToList();
        }
    }
}
