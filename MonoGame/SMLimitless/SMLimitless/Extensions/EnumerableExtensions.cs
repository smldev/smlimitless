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
    }
}
