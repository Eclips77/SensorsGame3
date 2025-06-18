using SensorsGame3.Entities.AbstractClasses;
using SensorsGame3.Factories;
using System;
using System.Collections.Generic;

namespace SensorsGame3.Entities.Agents
{
    public class OrganizationLeader : IranianAgent
    {
        private readonly Random random = new Random();

        public OrganizationLeader(List<SensorType> weaknesses)
            : base(weaknesses)
        {
            Rank = AgentRank.OrganizationLeader;
        }

        public override void OnTurnStart()
        {
            if (TurnCounter > 0 && TurnCounter % 3 == 0)
            {
                CounterAttack();
                if (random.NextDouble() < 0.5)
                {
                    ResetSensorsAndWeaknessList();
                    Console.WriteLine("Organization Leader  changed weaknesses!");
                }
            }
        }

        public override void CounterAttack()
        {
            if (AttachedSensors.Count == 0)
                return;
            int index = random.Next(AttachedSensors.Count);
            AttachedSensors.RemoveAt(index);
            Console.WriteLine("Organization Leader counterattacked and removed one sensor.");
        }

        public override void ResetSensorsAndWeaknessList()
        {
            AttachedSensors.Clear();
            WeaknessCombination = AgentFactory.GetRandomWeaknesses(AgentFactory.GetAllowedSensorTypes(Rank), GetRequiredSensors());
        }

        public override int Activate()
        {
            int matched = base.Activate();
            if (!IsExposed && matched >= WeaknessCombination.Count - 1)
            {
                ResetSensorsAndWeaknessList();
                Console.WriteLine("Organization Leader adapted its weaknesses!");
            }
            return matched;
        }

        private int GetRequiredSensors()
        {
            return 8;
        }
    }
}
