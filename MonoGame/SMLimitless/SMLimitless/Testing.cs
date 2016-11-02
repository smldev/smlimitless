//-----------------------------------------------------------------------
// <copyright file="Testing.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Diagnostics;

namespace SMLimitless
{
	/// <summary>
	///   A class used to test things. Subject to change.
	/// </summary>
	public static class Testing
	{
		/// <summary>
		///   Measures the time it takes to perform a given action once.
		/// </summary>
		/// <param name="action">The action to perform.</param>
		/// <returns>The time it took for the action to run.</returns>
		public static TimeSpan Time(Action action)
		{
			// Credit to Jon Skeet http://stackoverflow.com/questions/969290/c-exact-time-measurement-for-performance-testing
			Stopwatch stopwatch = Stopwatch.StartNew();
			action();
			stopwatch.Stop();
			return stopwatch.Elapsed;
		}

		/// <summary>
		///   Measures the time it takes to perform a given action a given number
		///   of times.
		/// </summary>
		/// <param name="action">The action to perform.</param>
		/// <param name="iterations">The number of times to perform it.</param>
		/// <returns>The time it took for the action to run.</returns>
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
