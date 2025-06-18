using SensorsGame3.Entities.AbstractClasses;

namespace SensorsGame3.Entities.Sensors
{
    public class ThermalSensor : Sensor
    {
        public ThermalSensor()
            : base("Thermal Sensor", SensorType.Thermal)
        {
        }

    }
}
