//-----------------------------------------------------------------------
// <copyright file="ListExtensions.cs" company="Chris Akridge">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMLimitless.Extensions
{
    /// <summary>
    /// Extends the List{T} generic class.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Adds an item to the list if the list does not already contain the item.
        /// </summary>
        /// <typeparam name="T">The type of the List{T}.</typeparam>
        /// <param name="list">The List{T} to add to.</param>
        /// <param name="item">The item to add to the List{T}.</param>
        public static void AddUnlessDuplicate<T>(this List<T> list, T item)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
            }
        }
    }
}
