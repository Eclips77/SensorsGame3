using SensorsGame3.Managers;
using SensorsGame3.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorsGame3
{
    public static class Runing
    {
        public static void Run()
        {
            try
            {
                var conn = Environment.GetEnvironmentVariable("GAME_DB_CONN") ??
                            "server=localhost;user id=root;password=;database=game";
                var playerDal = new PlayerDal(conn);

                bool exitProgram = false;
                while (!exitProgram)
                {
                    string user;
                    do
                    {
                        Console.Write("Enter username: ");
                        user = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(user))
                        {
                            Console.WriteLine("Username cannot be empty. Please try again.");
                        }
                    } while (string.IsNullOrWhiteSpace(user));

                    PlayerSession.Login(user);

                    try
                    {
                        playerDal.EnsurePlayerExists(user);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in EnsurePlayerExists: {ex.Message}");
                    }

                    bool logout = MenuManager.StartApplicationLoop();
                    if (!logout)
                    {
                        exitProgram = true;
                    }
                    PlayerSession.Logout();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($" error in Run: {ex.Message}");
            }
        }
    }
}
