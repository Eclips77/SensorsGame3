using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace SensorsGame3.Tools
{
    public class PlayerDal
    {
        private readonly string _connectionString;

        public PlayerDal(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void EnsurePlayerExists(string username)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "INSERT IGNORE INTO players(username, highest_rank_unlocked) VALUES(@user, 0);";
                        cmd.Parameters.AddWithValue("@user", username);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PlayerDal.EnsurePlayerExists: {ex.Message}");
            }
        }

        public List<string> GetAllUsernames()
        {
            var list = new List<string>();
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT username FROM players";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(reader.GetString("username"));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PlayerDal.GetAllUsernames: {ex.Message}");
            }
            return list;
        }

        public int GetHighestRank(string username)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT highest_rank_unlocked FROM players WHERE username=@user";
                        cmd.Parameters.AddWithValue("@user", username);
                        var result = cmd.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int rank))
                            return rank;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PlayerDal.GetHighestRank: {ex.Message}");
            }
            return 0;
        }
    }
}
