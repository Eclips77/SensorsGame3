using System;
using System.Collections.Generic;
using SensorsGame3.Factories;
using SensorsGame3.Tools;
using SensorsGame3.Enums;

namespace SensorsGame3.Managers
{
    public static class MenuManager
    {
        private static readonly Dictionary<int, AgentRank> agentOptions = new Dictionary<int, AgentRank>
        {
            { 1, AgentRank.FootSoldier },
            { 2, AgentRank.SquadLeader },
            { 3, AgentRank.SeniorCommander },
            { 4, AgentRank.OrganizationLeader }
        };

        private static readonly Dictionary<SensorType, int> sensorCosts = new Dictionary<SensorType, int>
        {
            { SensorType.Audio, 5 },
            { SensorType.Thermal, 7 },
            { SensorType.Pulse, 10 },
            { SensorType.Magnetic, 12 },
            { SensorType.Motion, 8 },
            { SensorType.Signal, 15 },
            { SensorType.Light, 20 },
            { SensorType.Jammer, 25 }
        };

        private static readonly Dictionary<int, SensorType> sensorMap = new Dictionary<int, SensorType>
        {
            { 1, SensorType.Audio },
            { 2, SensorType.Thermal },
            { 3, SensorType.Pulse },
            { 4, SensorType.Magnetic },
            { 5, SensorType.Motion },
            { 6, SensorType.Signal },
            { 7, SensorType.Light },
            { 8, SensorType.Jammer }
        };

        private static readonly Dictionary<AgentRank, List<SensorType>> allowedByRank = new Dictionary<AgentRank, List<SensorType>>
        {
            { AgentRank.FootSoldier, new List<SensorType> { SensorType.Audio, SensorType.Thermal } },
            { AgentRank.SquadLeader, new List<SensorType> { SensorType.Audio, SensorType.Thermal, SensorType.Pulse, SensorType.Magnetic, SensorType.Motion } },
            { AgentRank.SeniorCommander, new List<SensorType> { SensorType.Audio, SensorType.Thermal, SensorType.Pulse, SensorType.Magnetic, SensorType.Motion, SensorType.Signal } },
            { AgentRank.OrganizationLeader, new List<SensorType> { SensorType.Audio, SensorType.Thermal, SensorType.Pulse, SensorType.Magnetic, SensorType.Motion, SensorType.Signal, SensorType.Light, SensorType.Jammer } }
        };

        public static bool StartApplicationLoop()
        {
            bool exitApp = false;
            bool logout = false;
            while (!exitApp && !logout)
            {
                try
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("=== IRANIAN AGENT INVESTIGATION GAME ===");
                    Console.ResetColor();
                    Console.WriteLine("----------------------------------------");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("1. Start New Investigation");
                    Console.WriteLine("2. Start Parallel Investigation");
                    Console.WriteLine("3. View Game History");
                    Console.WriteLine("4. View Rankings");
                    Console.WriteLine("5. How To Play");
                    Console.WriteLine("6. Log Out");
                    Console.WriteLine("7. Exit");
                    Console.ResetColor();
                    Console.WriteLine("----------------------------------------");

                    int choice = InputHelper.GetNumber("Enter your choice:");

                    switch (choice)
                    {
                        case 1:
                            DisplayAgentSelectionMenu();
                            break;
                        case 2:
                            DisplayParallelInvestigationMenu();
                            break;
                        case 3:
                            if (PlayerSession.IsAdmin)
                            {
                                DisplayAdminHistoryMenu();
                            }
                            else
                            {
                                GameHistory.Instance.DisplayHistory(PlayerSession.Username);
                            }
                            break;
                        case 4:
                            RankingManager.DisplayRankings();
                            break;
                        case 5:
                            DisplayInstructions();
                            break;
                        case 6:
                            logout = true;
                            break;
                        case 7:
                            exitApp = true;
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in StartApplicationLoop: {ex.Message}");
                }
            }
            Console.WriteLine("Thank you for playing!");
            return logout;
        }

        private static void DisplayAgentSelectionMenu()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("=== IRANIAN AGENT INVESTIGATION GAME ===");
                Console.WriteLine("Choose your target:");
                Console.WriteLine("----------------------------------------");

                foreach (var option in agentOptions)
                {
                    Console.WriteLine($"{option.Key}. {option.Value}");
                    string required = "2";
                    if (option.Value == AgentRank.SquadLeader) required = "4";
                    else if (option.Value == AgentRank.SeniorCommander) required = "6";
                    else if (option.Value == AgentRank.OrganizationLeader) required = "8";
                    Console.WriteLine($"   Required Sensors: {required}");
                    Console.WriteLine($"   Available Sensors: {string.Join(", ", allowedByRank[option.Value])}");
                    Console.WriteLine();
                }

                int choice = InputHelper.GetNumber("Enter target number (1-4):");
                if (!agentOptions.ContainsKey(choice))
                {
                    Console.WriteLine("Invalid choice. Returning to main menu.");
                    return;
                }

                AgentRank selectedRank = agentOptions[choice];
                Console.WriteLine("Choose difficulty: 1.Easy 2.Medium 3.Hard");
                int diffChoice = InputHelper.GetNumber("Enter choice:");
                Difficulty diff = Difficulty.Medium;
                if (diffChoice == 1) diff = Difficulty.Easy;
                else if (diffChoice == 3) diff = Difficulty.Hard;

                var agent = AgentFactory.CreateAgent(selectedRank);

                var investigation = new InvestigationManager(agent, diff);
                investigation.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DisplayAgentSelectionMenu: {ex.Message}");
            }
        }

        private static void DisplayAdminHistoryMenu()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("=== HISTORY MENU ===");
                Console.WriteLine("1. View All History");
                Console.WriteLine("2. View History By Player");
                Console.WriteLine("3. Back");

                int choice = InputHelper.GetNumber("Enter your choice:");

                switch (choice)
                {
                    case 1:
                        GameHistory.Instance.DisplayHistory();
                        break;
                    case 2:
                        var conn = Environment.GetEnvironmentVariable("GAME_DB_CONN") ??
                                    "server=localhost;user id=root;password=;database=game";
                        var playerDal = new PlayerDal(conn);
                        var users = playerDal.GetAllUsernames();
                        for (int i = 0; i < users.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}. {users[i]}");
                        }
                        int userChoice = InputHelper.GetNumber("Select player number:");
                        if (userChoice > 0 && userChoice <= users.Count)
                            GameHistory.Instance.DisplayHistory(users[userChoice - 1]);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DisplayAdminHistoryMenu: {ex.Message}");
            }
        }

        public static void PrintSensorOptions(AgentRank rank)
        {
            Console.WriteLine("\nAvailable Sensors for this target:");
            Console.WriteLine("----------------------------------------");
            foreach (var kvp in sensorMap)
            {
                if (allowedByRank.ContainsKey(rank) && allowedByRank[rank].Contains(kvp.Value))
                {
                    Console.WriteLine($"{kvp.Key}. {kvp.Value}");
                }
            }
        }

        public static SensorType GetSensorTypeByChoice(int choice)
        {
            if (!sensorMap.ContainsKey(choice))
                throw new ArgumentException("Invalid sensor choice! Please select a valid number.");

            return sensorMap[choice];
        }

        public static int GetSensorCost(SensorType type)
        {
            return sensorCosts.ContainsKey(type) ? sensorCosts[type] : 0;
        }

        private static void DisplayParallelInvestigationMenu()
        {
            var a1 = AgentFactory.CreateAgent(AgentRank.FootSoldier);
            var a2 = AgentFactory.CreateAgent(AgentRank.SquadLeader);
            var manager = new ParallelInvestigationManager(a1, a2, Difficulty.Medium);
            manager.Run();
        }

        private static void DisplayInstructions()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=== HOW TO PLAY ===");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("1. Select \"Start New Investigation\" to choose an agent rank.");
            Console.WriteLine("   FootSoldier requires 2 sensors, SquadLeader 4, SeniorCommander 6 and OrganizationLeader 8.");
            Console.WriteLine("2. Each turn you have limited time to pick a sensor. A countdown will show the remaining seconds.");
            Console.WriteLine("3. Sensors cost credits from your budget. Match the agent's weaknesses before running out of attempts.");
            Console.WriteLine("4. Answer the pre-mission trivia question correctly to earn bonus credits.");
            Console.WriteLine();
            Console.WriteLine("Press any key to return to the menu...");
            Console.ReadKey();
        }
    }
}
