using System;

namespace SensorsGame3.Tools
{
    public static class InputHelper
    {
        public static int GetNumber(string message)
        {
            Console.Write(message + " ");
            string input = Console.ReadLine();
            int number;
            while (!int.TryParse(input, out number))
            {
                Console.Write("Invalid input. Try again: ");
                input = Console.ReadLine();
            }
            return number;
        }

        public static int GetNumberWithTimeout(string message, int seconds, out bool timedOut)
        {
            timedOut = false;
            Console.Write(message + " ");
            var input = string.Empty;
            DateTime end = DateTime.Now.AddSeconds(seconds);
            int lastRemaining = -1;

            while (DateTime.Now < end)
            {
                while (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Enter)
                    {
                        Console.WriteLine();
                        if (int.TryParse(input, out int result))
                            return result;
                        Console.Write("Invalid input. Try again: ");
                        input = string.Empty;
                        end = DateTime.Now.AddSeconds(seconds);
                        continue;
                    }
                    else if (key.Key == ConsoleKey.Backspace)
                    {
                        if (input.Length > 0)
                        {
                            input = input.Remove(input.Length - 1);
                            int left = Console.CursorLeft;
                            if (left > 0)
                            {
                                Console.CursorLeft = left - 1;
                                Console.Write(" ");
                                Console.CursorLeft = left - 1;
                            }
                        }
                    }
                    else
                    {
                        input += key.KeyChar;
                        Console.Write(key.KeyChar);
                    }
                }

                int remaining = (int)Math.Ceiling((end - DateTime.Now).TotalSeconds);
                if (remaining != lastRemaining)
                {
                    int curLeft = Console.CursorLeft;
                    int curTop = Console.CursorTop;
                    Console.SetCursorPosition(0, curTop);
                    Console.Write($"{message} {input} ({remaining}s left)    ");
                    Console.SetCursorPosition(curLeft, curTop);
                    lastRemaining = remaining;
                }

                System.Threading.Thread.Sleep(100);
            }

            Console.WriteLine();
            timedOut = true;
            return -1;
        }
    }
}
