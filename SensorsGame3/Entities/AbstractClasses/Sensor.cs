using System.Collections.Generic;
using SensorsGame3.Tools;

namespace SensorsGame3.Entities.AbstractClasses
{
    public abstract class Sensor
    {
        public string Name { get; protected set; }
        public SensorType Type { get; protected set; }

        protected Sensor(string name, SensorType type)
        {
            Name = name;
            Type = type;
        }

        public virtual bool Activate(List<SensorType> weaknesses)
        {
            if (!WeatherService.IsSensorEffective(Type))
                return false;

            return weaknesses.Contains(Type);
        }
    }

}

