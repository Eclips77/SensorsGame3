using SensorsGame3.Entities.AbstractClasses;
using System.Collections.Generic;

namespace SensorsGame3.Entities.Sensors
{
    public class MagneticSensor : Sensor
    {
        public MagneticSensor()
            : base("Magnetic Sensor", SensorType.Magnetic)
        {
        }

        public override bool Activate(List<SensorType> weaknesses)
        {
            return weaknesses.Contains(Type);
        }
    }
} 

