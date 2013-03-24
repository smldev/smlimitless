using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using SMLimitless.Extensions;
using SMLimitless.Physics;
using SMLimitless.Sprites;

namespace SMLimitless
{
    public static class Testing
    {
        // Credit to Jon Skeet
        // http://stackoverflow.com/questions/969290/c-exact-time-measurement-for-performance-testing
        public static TimeSpan Time(Action action)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            action();
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        public static TimeSpan Time(Action action, int iterations)
        {
            int i;
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (i = 0; i < iterations; i++)
            {
                action();
            }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }
    }
}
