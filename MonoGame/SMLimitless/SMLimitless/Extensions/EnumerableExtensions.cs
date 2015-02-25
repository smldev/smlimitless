//-----------------------------------------------------------------------
// <copyright file="EnumerableExtensions.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMLimitless.Extensions
{
    /// <summary>
    /// Provides extensions for the enumerable classes and interfaces.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Enumerates over every item in an enumerable and performs a given action on it.
        /// </summary>
        /// <typeparam name="T">The type of the items to enumerate over.</typeparam>
        /// <param name="items">The enumerable containing the items.</param>
        /// <param name="action">The action to perform on the items.</param>
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (T item in items)
            {
                action(item);
            }
        }

		/// <summary>
		/// Eagerly separates an enumerable into lists of sublists when given a selector function.
		/// Each sublist contains all the items that have the same selector value.
		/// </summary>
		/// <typeparam name="T">The type of the items.</typeparam>
		/// <typeparam name="TSelector">The type of the selector function.</typeparam>
		/// <param name="items">A collection of items.</param>
		/// <param name="value">A selector function that is used to separate the enumerable.</param>
		/// <returns>A list of lists, each list containing all the items for which the selector function returned the same value.</returns>
		public static List<List<T>> EagerSeparate<T, TSelector>(this IEnumerable<T> items, Func<T, TSelector> value)
		{
			List<List<T>> result = new List<List<T>>();
			List<T> current = new List<T>();
			TSelector lastValue = default(TSelector);

			items = items.OrderBy(i => value(i));

			foreach (T item in items)
			{
				TSelector currentValue = value(item);

				if (!currentValue.Equals(lastValue))
				{
					// Finish the list and start another
					result.Add(current);
					current = new List<T>();
				}
				current.Add(item);
				lastValue = currentValue;
			}

			return result;
		}
    }
}
