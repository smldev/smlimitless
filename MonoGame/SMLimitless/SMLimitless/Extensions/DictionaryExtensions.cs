using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMLimitless.Extensions
{
	/// <summary>
	/// Contains extensions methods for the <see cref="Dictionary{TKey, TValue}"/> type.
	/// </summary>
	public static class DictionaryExtensions
	{
	/// <summary>
	/// Removes all the keys and their values that match a predicate from a given dictionary.
	/// </summary>
	/// <typeparam name="TKey">The type of the dictionary's keys.</typeparam>
	/// <typeparam name="TValue">The type of the dictionary's values.</typeparam>
	/// <param name="dictionary">The dictionary from which to remove keys.</param>
	/// <param name="predicate">The predicate by which all keys are checked.</param>
		public static void RemoveAll<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Func<TValue, bool> predicate)
		{
			// Credit to aku http://stackoverflow.com/a/469262/2709212
			var keys = dictionary.Keys.Where(k => predicate(dictionary[k])).ToList();
			foreach (var key in keys)
			{
				dictionary.Remove(key);
			}
		}
	}
}
