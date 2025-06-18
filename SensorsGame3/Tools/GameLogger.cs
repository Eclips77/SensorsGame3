using System;
using System.Collections.Generic;

namespace SensorsGame3.Tools
{
    public static class GameLogger
    {
        private static readonly List<string> _lines = new List<string>();
        public static void Clear() => _lines.Clear();
        public static void Log(string line)
        {
            _lines.Add(line);
        }
        public static void Save(string fileName)
        {

        }
    }
}
