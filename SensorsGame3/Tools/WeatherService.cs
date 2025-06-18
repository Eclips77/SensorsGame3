using System;
using SensorsGame3.Entities.AbstractClasses;

namespace SensorsGame3.Tools
{
    public enum WeatherCondition
    {
        Clear,
        Rain,
        Fog,
        Storm
    }

    public static class WeatherService
    {
        private static readonly Random rnd = new Random();
        public static WeatherCondition CurrentWeather { get; private set; }

        public static void GenerateWeather()
        {
            Array values = Enum.GetValues(typeof(WeatherCondition));
            CurrentWeather = (WeatherCondition)values.GetValue(rnd.Next(values.Length));
        }

        public static bool IsSensorEffective(SensorType type)
        {
            return true;
        }
    }
}
