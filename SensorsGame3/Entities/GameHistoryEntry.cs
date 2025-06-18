using System;
using System.Collections.Generic;

namespace SensorsGame3.Entities
{
    public class GameHistoryEntry
    {
        public string Username { get; set; }
        public string AgentType { get; set; }
        public List<SensorType> WeaknessCombination { get; set; }
        public List<SensorType> UsedSensors { get; set; }
        public List<SensorType> CorrectSensors { get; set; }
        public int TurnsTaken { get; set; }
        public int Score { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Victory { get; set; }
    }
}
