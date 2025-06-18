using SensorsGame3.Entities.AbstractClasses;
using SensorsGame3.Entities.Sensors;
using System;

namespace SensorsGame3.Factories
{
    public static class SensorsFactory
    {
        public static Sensor CreateSensor(SensorType type)
        {
            switch (type)
            {
                case SensorType.Thermal:
                    return new ThermalSensor();
                case SensorType.Audio:
                    return new AudioSensor();
                case SensorType.Pulse:
                    return new PulseSensor();
                case SensorType.Motion:
                    return new MotionSensor();
                case SensorType.Magnetic:
                    return new MagneticSensor();
                case SensorType.Signal:
                    return new SignalSensor();
                case SensorType.Light:
                    return new LightSensor();
                case SensorType.Jammer:
                    return new JammerSensor();
                default:
                    throw new ArgumentException($"Unknown Sensor Type: {type}");
            }
        }
    }
}
