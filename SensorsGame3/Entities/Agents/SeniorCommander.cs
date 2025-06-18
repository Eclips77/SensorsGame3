using SensorsGame3.Entities.AbstractClasses;
using System;
using System.Collections.Generic;

namespace SensorsGame3.Entities.Agents
{
    public class SeniorCommander : IranianAgent
    {
        private readonly Random random = new Random();

        public SeniorCommander(List<SensorType> weaknesses)
            : base(weaknesses)
        {
            Rank = AgentRank.SeniorCommander;
        }

        public override void OnTurnStart()
        {
            if (TurnCounter > 0 && TurnCounter % 3 == 0)
            {
                CounterAttack();
            }
        }

        public override void CounterAttack()
        {
            int removeCount = Math.Min(2, AttachedSensors.Count);
            for (int i = 0; i < removeCount; i++)
            {
                int index = random.Next(AttachedSensors.Count);
                AttachedSensors.RemoveAt(index);
            }
            if (removeCount > 0)
            {
                Console.WriteLine("Senior Commander counterattacked and removed two sensors.");
            }
        }
    }
}
