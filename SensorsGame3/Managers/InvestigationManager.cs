using SensorsGame3.Entities.AbstractClasses;
using SensorsGame3.Factories;
using SensorsGame3.Tools;
using SensorsGame3.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
namespace SensorsGame3.Managers
{
    public class InvestigationManager
    {
        private IranianAgent currentAgent;
        private bool gameRunning = true;
        private int attempts;
        private readonly List<SensorType> _currentSessionUsedSensors;
        private readonly Difficulty _difficulty;
        private int _budget;
        private readonly int _timeLimit;
        private bool _comboGuessed;

        public InvestigationManager(IranianAgent agent, Difficulty difficulty = Difficulty.Medium)
        {
            currentAgent = agent;
            _difficulty = difficulty;
            attempts = 0;
            _currentSessionUsedSensors = new List<SensorType>();
            _budget = difficulty == Difficulty.Easy ? 150 : difficulty == Difficulty.Medium ? 100 : 70;
            _timeLimit = difficulty == Difficulty.Hard ? 20 : 30;
            _comboGuessed = false;
        }

        public void StartGame()
        {
            Console.Clear();
            Console.WriteLine("=== Welcome to the Investigation Game ===");
            Console.WriteLine($"Target: {currentAgent.Rank}");
            Console.WriteLine($"Required Sensors: {GetRequiredSensorsCount()}");
            Console.WriteLine("----------------------------------------");
            //GameLogger.Clear();
            WeatherService.GenerateWeather();
            Console.WriteLine($"Weather Today: {WeatherService.CurrentWeather}");
            Console.WriteLine($"Starting Budget: {_budget}");

            int maxAttemptsForAgent = (currentAgent.Rank == AgentRank.FootSoldier) ? 5 : 15;

            try
            {
                while (gameRunning && attempts < maxAttemptsForAgent)
                {
                    attempts++;
                    currentAgent.OnTurnStart();
                    Console.WriteLine($"\nAttempt {attempts} of {maxAttemptsForAgent}");
                    Console.WriteLine("----------------------------------------");

                    MenuManager.PrintSensorOptions(currentAgent.Rank);

                    bool timed;
                    int choice = InputHelper.GetNumberWithTimeout("\nEnter sensor number (0 to guess combo):", _timeLimit, out timed);
                    if (timed)
                    {
                        Console.WriteLine("Time ran out for this turn!");
                        continue;
                    }

                    if (choice == 0 && !_comboGuessed)
                    {
                        HandleComboGuess();
                        continue;
                    }

                    SensorType selectedType = MenuManager.GetSensorTypeByChoice(choice);

                    if (_currentSessionUsedSensors.Contains(selectedType))
                    {
                        Console.WriteLine("Sensor type already used this game.");
                        continue;
                    }

                    int cost = MenuManager.GetSensorCost(selectedType);
                    if (_budget - cost < 0)
                    {
                        Console.WriteLine("Not enough budget for this sensor.");
                        continue;
                    }
                    _budget -= cost;
                    Console.WriteLine($"Budget remaining: {_budget}");
                    
                    try
                    {
                        Sensor sensor = SensorsFactory.CreateSensor(selectedType);
                        currentAgent.AddSensor(sensor);
                        _currentSessionUsedSensors.Add(selectedType);
                        //GameLogger.Log($"Turn {attempts}: Used {selectedType}");

                        int correct = currentAgent.Activate();
                        int required = GetRequiredSensorsCount();
                        //GameLogger.Log($"Matched {correct}/{required}");
                        Console.WriteLine($"\nMatched sensors: {correct}/{required}");


                        if (currentAgent.CheckIfExposed())
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("\n=== SUCCESS! ===");
                            Console.ResetColor();
                            Console.WriteLine($"Agent {currentAgent.Rank} has been exposed!");
                            Console.WriteLine($"It took you {attempts} attempts.");
                            //GameLogger.Log("Game won");

                            int baseScore = currentAgent.Rank == AgentRank.FootSoldier ? 100 : 200;
                            int score = baseScore / attempts;
                            GameHistory.Instance.AddSession(new GameSession(
                                PlayerSession.Username,
                                currentAgent.Rank,
                                score,
                                _currentSessionUsedSensors,
                                attempts,
                                true,
                                currentAgent.GetWeaknesses()));

                            try
                            {
                                var conn = Environment.GetEnvironmentVariable("GAME_DB_CONN") ??
                                            "server=localhost;user id=root;password=;database=game";
                                var statsDal = new AgentDal(conn);
                                statsDal.UpdatePlayerStats(PlayerSession.Username, (int)currentAgent.Rank);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error updating player stats: {ex.Message}");
                            }

                            Console.WriteLine("\nPress any key to continue...");
                            Console.ReadKey();

                            gameRunning = false;
                        }
                        else if (attempts >= maxAttemptsForAgent)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\n=== GAME OVER ===");
                            Console.ResetColor();
                            Console.WriteLine("You've run out of attempts!");
                            Console.WriteLine($"The {currentAgent.Rank} has escaped.");
                            //GameLogger.Log("Game lost");

                            GameHistory.Instance.AddSession(new GameSession(
                                PlayerSession.Username,
                                currentAgent.Rank,
                                0,
                                _currentSessionUsedSensors,
                                attempts,
                                false,
                                currentAgent.GetWeaknesses()));

                            Console.WriteLine("\nPress any key to continue...");
                            Console.ReadKey();

                            gameRunning = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\nError: {ex.Message}");
                        Console.WriteLine("Please try again.");
                        attempts--; 
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError in StartGame: {ex.Message}");
                Console.WriteLine("Game cannot continue. Please restart.");
            }
        }

        private int GetRequiredSensorsCount()
        {
            switch (currentAgent.Rank)
            {
                case AgentRank.FootSoldier:
                    return 2;
                case AgentRank.SquadLeader:
                    return 4;
                case AgentRank.SeniorCommander:
                    return 6;
                case AgentRank.OrganizationLeader:
                    return 8;
                default:
                    return 2;
            }
        }

        public void Run()
        {
            StartGame();
        }


        private void HandleComboGuess()
        {
            Console.WriteLine("Enter your guess for the full weakness combo separated by commas:");
            string input = Console.ReadLine();
            var parts = input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var guess = new List<SensorType>();
            foreach (var p in parts)
            {
                if (Enum.TryParse(p.Trim(), true, out SensorType t))
                    guess.Add(t);
            }
            var actual = currentAgent.GetWeaknesses();
            if (guess.Count == actual.Count && !guess.Except(actual).Any())
            {
                Console.WriteLine("Amazing! You exposed the agent!");
                //GameLogger.Log("Full combo guessed correctly");
                currentAgent.ResetSensorsAndWeaknessList();
                typeof(IranianAgent).GetProperty("IsExposed").SetValue(currentAgent, true);
            }
            else
            {
                Console.WriteLine("Wrong guess!");
                //GameLogger.Log("Failed combo guess");
            }
            _comboGuessed = true;
        }
    }
}
