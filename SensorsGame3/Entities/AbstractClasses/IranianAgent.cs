using System.Collections.Generic;
using System;
using SensorsGame3.Enums;

namespace SensorsGame3.Entities.AbstractClasses

{
    public abstract class IranianAgent
    {
        public AgentRank Rank { get; protected set; }
        protected List<SensorType> WeaknessCombination;
        protected List<Sensor> AttachedSensors;
        public bool IsExposed { get; protected set; }
        public AgentMood Mood { get; protected set; }
        protected int TurnCounter;
        protected int ActivateCounter;

        public List<SensorType> GetWeaknesses()
        {
            return new List<SensorType>(WeaknessCombination);
        }

        protected IranianAgent(List<SensorType> weaknesses)
        {
            WeaknessCombination = weaknesses;
            AttachedSensors = new List<Sensor>();
            IsExposed = false;
            TurnCounter = 0;
            ActivateCounter = 0;
            Mood = AgentMood.Calm;
        }

        public void AddSensor(Sensor sensor)
        {
            AttachedSensors.Add(sensor);
        }

        public virtual int Activate()
        {
            var matchedTypes = new HashSet<SensorType>();
            foreach (Sensor sensor in AttachedSensors)
            {
                if (sensor.Activate(WeaknessCombination))
                    matchedTypes.Add(sensor.Type);
            }

            double ratio = (double)matchedTypes.Count / WeaknessCombination.Count;
            if (ratio >= 1)
                Mood = AgentMood.Panicked;
            else if (ratio >= 0.75)
                Mood = AgentMood.Nervous;
            else if (ratio >= 0.5)
                Mood = AgentMood.Alert;
            else
                Mood = AgentMood.Calm;

            ConsoleColor prev = Console.ForegroundColor;
            ConsoleColor moodColor = ConsoleColor.White;
            switch (Mood)
            {
                case AgentMood.Calm:
                    moodColor = ConsoleColor.Green;
                    break;
                case AgentMood.Alert:
                    moodColor = ConsoleColor.Yellow;
                    break;
                case AgentMood.Nervous:
                    moodColor = ConsoleColor.DarkYellow;
                    break;
                case AgentMood.Panicked:
                    moodColor = ConsoleColor.Red;
                    break;
            }
            Console.ForegroundColor = moodColor;
            Console.WriteLine($"Agent mood: {Mood}");
            Console.ForegroundColor = prev;

            if (matchedTypes.Count == WeaknessCombination.Count)
                IsExposed = true;

            TurnCounter++;
            ActivateCounter++;
            return matchedTypes.Count;
        }

        public bool CheckIfExposed()
        {
            return IsExposed;
        }

        public virtual void OnTurnStart() { }
        public virtual void CounterAttack() { }
        public virtual void ResetSensorsAndWeaknessList() { }
    }
}
