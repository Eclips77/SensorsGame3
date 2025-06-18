using System;
using MySql.Data.MySqlClient;

namespace SensorsGame3.Tools
{
    public class AgentDal
    {
        private readonly string _connectionString;

        public AgentDal(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void UpdatePlayerStats(string username, int highestRank)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO players(username, highest_rank_unlocked) VALUES(@user, @rank) " +
                                         "ON DUPLICATE KEY UPDATE highest_rank_unlocked = GREATEST(highest_rank_unlocked, @rank)";
                        cmd.Parameters.AddWithValue("@user", username);
                        cmd.Parameters.AddWithValue("@rank", highestRank);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AgentDal.UpdatePlayerStats: {ex.Message}");
            }
        }
    }
}
