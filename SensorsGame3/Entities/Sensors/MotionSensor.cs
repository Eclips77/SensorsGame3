using SensorsGame3.Entities.AbstractClasses;
using System.Collections.Generic;

namespace SensorsGame3.Entities.Sensors
{
    public class MotionSensor : Sensor
    {
        private int usesLeft;

        public MotionSensor()
            : base("Motion Sensor", SensorType.Motion)
        {
            usesLeft = 3;
        }

        public override bool Activate(List<SensorType> weaknesses)
        {
            if (IsBroken())
                return false;

            usesLeft--;

            return weaknesses.Contains(Type);
        }

        public bool IsBroken()
        {
            return usesLeft <= 0;
        }
    }
}
