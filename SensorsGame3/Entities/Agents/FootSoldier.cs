using System.Collections.Generic;
using SensorsGame3.Entities.AbstractClasses;


namespace SensorsGame3.Entities.Agents
{
    public class FootSoldier : IranianAgent
    {
        public FootSoldier(List<SensorType> weaknesses)
            : base(weaknesses)
        {
            Rank = AgentRank.FootSoldier;
        }

    }
}
