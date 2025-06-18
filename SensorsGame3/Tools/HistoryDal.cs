using SensorsGame3.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace SensorsGame3.Tools
{
    public class HistoryDal
    {
        private readonly string _connectionString;

        public HistoryDal(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Insert(GameHistoryEntry entry)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO game_history(username, agent_type, weakness_combo, used_sensors, correct_sensors, turns_taken, score, victory, timestamp) " +
                                         "VALUES(@user, @agent, @weak, @used, @correct, @turns, @score, @victory, @time)";
                        cmd.Parameters.AddWithValue("@user", entry.Username);
                        cmd.Parameters.AddWithValue("@agent", entry.AgentType);
                        cmd.Parameters.AddWithValue("@weak", string.Join(",", entry.WeaknessCombination));
                        cmd.Parameters.AddWithValue("@used", string.Join(",", entry.UsedSensors));
                        cmd.Parameters.AddWithValue("@correct", string.Join(",", entry.CorrectSensors));
                        cmd.Parameters.AddWithValue("@turns", entry.TurnsTaken);
                        cmd.Parameters.AddWithValue("@score", entry.Score);
                        cmd.Parameters.AddWithValue("@victory", entry.Victory);
                        cmd.Parameters.AddWithValue("@time", entry.Timestamp);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HistoryDal.Insert: {ex.Message}");
            }
        }

        private static List<SensorType> ParseSensorList(string data)
        {
            var list = new List<SensorType>();
            if (string.IsNullOrWhiteSpace(data))
                return list;

            foreach (var part in data.Split(','))
            {
                if (Enum.TryParse(part, out SensorType t))
                    list.Add(t);
            }
            return list;
        }

        public List<GameHistoryEntry> GetAll()
        {
            var list = new List<GameHistoryEntry>();
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("SELECT * FROM game_history", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var entry = new GameHistoryEntry
                            {
                                Username = reader.GetString("username"),
                                AgentType = reader.GetString("agent_type"),
                                WeaknessCombination = ParseSensorList(reader.GetString("weakness_combo")),
                                UsedSensors = ParseSensorList(reader.GetString("used_sensors")),
                                CorrectSensors = ParseSensorList(reader.GetString("correct_sensors")),
                                TurnsTaken = reader.GetInt32("turns_taken"),
                                Score = reader.GetInt32("score"),
                                Victory = reader.GetBoolean("victory"),
                                Timestamp = reader.GetDateTime("timestamp")
                            };
                            list.Add(entry);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HistoryDal.GetAll: {ex.Message}");
            }
            return list;
        }

        public List<GameHistoryEntry> GetByUser(string username)
        {
            var list = new List<GameHistoryEntry>();
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("SELECT * FROM game_history WHERE username=@user", conn))
                    {
                        cmd.Parameters.AddWithValue("@user", username);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var entry = new GameHistoryEntry
                                {
                                    Username = reader.GetString("username"),
                                    AgentType = reader.GetString("agent_type"),
                                    WeaknessCombination = ParseSensorList(reader.GetString("weakness_combo")),
                                    UsedSensors = ParseSensorList(reader.GetString("used_sensors")),
                                    CorrectSensors = ParseSensorList(reader.GetString("correct_sensors")),
                                    TurnsTaken = reader.GetInt32("turns_taken"),
                                    Score = reader.GetInt32("score"),
                                    Victory = reader.GetBoolean("victory"),
                                    Timestamp = reader.GetDateTime("timestamp")
                                };
                                list.Add(entry);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HistoryDal.GetByUser: {ex.Message}");
            }
            return list;
        }
    }
}
