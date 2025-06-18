using SensorsGame3.Entities.AbstractClasses;
using System;
using System.Collections.Generic;

namespace SensorsGame3.Entities.Agents
{
    public class SquadLeader : IranianAgent
    {
        private readonly Random random = new Random();

        public SquadLeader(List<SensorType> weaknesses)
            : base(weaknesses)
        {
            Rank = AgentRank.SquadLeader;
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
            if (AttachedSensors.Count == 0)
                return;

            int index = random.Next(AttachedSensors.Count);
            AttachedSensors.RemoveAt(index);

            Console.WriteLine("Squad Leader performed a counterattack and removed one sensor.");
        }
    }
}
