using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Aethra.RayTracer.Utils
{
    public static class Timer
    {
        private static readonly Stack<(int id, Stopwatch stopwatch)> Stopwatches = new Stack<(int id, Stopwatch stopwatch)>();
        private static int _currentId = 1;

        public static void Start()
        {
            var stopwatch = new Stopwatch();
            _currentId++;
            Stopwatches.Push((_currentId, stopwatch));
            stopwatch.Start();
        }

        public static void Stop(string prefix = "Execution Time", Action<string>? displayResult = null)
        {
            if (Stopwatches.Count == 0)
            {
                throw new IncorrectMethodOrderException(nameof(Start));    
            }
            
            var pop = Stopwatches.Pop();
            if (pop.id == _currentId)
            {
                _currentId--;
                var stopwatch = pop.stopwatch;
                stopwatch.Stop();
                // Get the elapsed time as a TimeSpan value.
                var ts = stopwatch.Elapsed;

                string elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds:000}   -   {ts.Ticks} Ticks";
                Console.WriteLine(prefix + " " + elapsedTime);
                displayResult?.Invoke(prefix + " " + elapsedTime);
            }
            else
            {
                throw new IncorrectMethodOrderException(nameof(Start));
            }
        }

        public static void Reset()
        {
            _currentId = 0;
            Stopwatches.Clear();
        }
    }
}