using System;
using System.Collections;
using System.Collections.Generic;

namespace SMLimitless.Collections
{
	/// <summary>
	///   Represents a strongly-typed, read-only dictionaries mapping unique keys
	///   to values.
	/// </summary>
	/// <typeparam name="TKey">The type of the keys.</typeparam>
	/// <typeparam name="TValue">The type of the values.</typeparam>
	public sealed class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		// Credit to Thomas Levesque (http://stackoverflow.com/a/1269311/2709212)

		/// <summary>
		///   An internal dictionary which contains the values exposed as read-only.
		/// </summary>
		private readonly IDictionary<TKey, TValue> _dictionary;

		/// <summary>
		///   Gets the number of items in this dictionary.
		/// </summary>
		public int Count => _dictionary.Count;

		/// <summary>
		///   Gets a value indicating whether this dictionary is read-only.
		/// </summary>
		public bool IsReadOnly => true;

		/// <summary>
		///   Gets a collection of all the keys in this dictionary.
		/// </summary>
		public ICollection<TKey> Keys => _dictionary.Keys;

		/// <summary>
		///   Gets a collection of the values in this dictionary.
		/// </summary>
		public ICollection<TValue> Values => _dictionary.Values;

		/// <summary>
		///   Gets a value from this dictionary for a given key. Attempting to
		///   set a value will throw a NotSupportedException.
		/// </summary>
		/// <param name="key">The key for which to get the value.</param>
		/// <returns>The value for the given key.</returns>
		TValue IDictionary<TKey, TValue>.this[TKey key]
		{
			get
			{
				return this[key];
			}
			set
			{
				throw ReadOnlyException();
			}
		}

		/// <summary>
		///   Gets a value from this dictionary for a given key.
		/// </summary>
		/// <param name="key">The key for which to get the value.</param>
		/// <returns>The value for the given key.</returns>
		public TValue this[TKey key] => _dictionary[key];

		/// <summary>
		///   Initializes a new instance of the <see
		///   cref="ReadOnlyDictionary{TKey, TValue}" /> type.
		/// </summary>
		public ReadOnlyDictionary()
		{
			_dictionary = new Dictionary<TKey, TValue>();
		}

		/// <summary>
		///   Initializes a new instance of the <see
		///   cref="ReadOnlyDictionary{TKey, TValue}" /> type.
		/// </summary>
		/// <param name="dictionary">The dictionary to wrap as read-only.</param>
		public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
		{
			_dictionary = dictionary;
		}

		/// <summary>
		///   Determines if this dictionary contains a given key/value pair.
		/// </summary>
		/// <param name="item">The key/value pair to search for.</param>
		/// <returns>
		///   True if the given key/value pair is present in the dictionary,
		///   False if it is not.
		/// </returns>
		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			return _dictionary.Contains(item);
		}

		/// <summary>
		///   Determines if this dictionary contains a given key.
		/// </summary>
		/// <param name="key">The key to search for.</param>
		/// <returns>
		///   True if the dictionary contains this key, false if it does not.
		/// </returns>
		public bool ContainsKey(TKey key)
		{
			return _dictionary.ContainsKey(key);
		}

		/// <summary>
		///   Copies the key-value pairs in this dictionary to an array of
		///   key-value pairs at a given index.
		/// </summary>
		/// <param name="array">The array to copy to.</param>
		/// <param name="arrayIndex">
		///   The index in the array to start copying at.
		/// </param>
		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			_dictionary.CopyTo(array, arrayIndex);
		}

		/// <summary>
		///   Gets an enumerator over every key-value pair in this dictionary.
		/// </summary>
		/// <returns>An enumerator for this dictionary.</returns>
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return _dictionary.GetEnumerator();
		}

		/// <summary>
		///   To implement the <see cref="ICollection{T}" /> interface, this
		///   method is required. However, it only throws a NotSupportedException.
		/// </summary>
		/// <param name="item">The item to add.</param>
		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		{
			throw ReadOnlyException();
		}

		/// <summary>
		///   To implement the <see cref="ICollection{T}" /> interface, this
		///   method is required. However, it only throws a NotSupportedException.
		/// </summary>
		void ICollection<KeyValuePair<TKey, TValue>>.Clear()
		{
			throw ReadOnlyException();
		}

		/// <summary>
		///   To implement the <see cref="ICollection{T}" /> interface, this
		///   method is required. However, it only throws a NotSupportedException.
		/// </summary>
		/// <param name="item">The item to add.</param>
		/// <returns>Nothing.</returns>
		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			throw ReadOnlyException();
		}

		/// <summary>
		///   To implement the <see cref="IDictionary{TKey, TValue}" />
		///   interface, this method is required. However, it only throws a NotSupportedException.
		/// </summary>
		/// <param name="key">The key to add.</param>
		/// <param name="value">The value to add.</param>
		void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
		{
			throw ReadOnlyException();
		}

		/// <summary>
		///   To implement the <see cref="IDictionary{TKey, TValue}" />
		///   interface, this method is required. However, it only throws a NotSupportedException.
		/// </summary>
		/// <param name="key">The key to remove.</param>
		/// <returns>Nothing.</returns>
		bool IDictionary<TKey, TValue>.Remove(TKey key)
		{
			throw ReadOnlyException();
		}

		/// <summary>
		///   Gets a non-generic enumerator over every key-value pair in this dictionary.
		/// </summary>
		/// <returns>A non-generic enumerator for this dictionary.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		///   Attempts to get a value from this dictionary for a given key.
		/// </summary>
		/// <param name="key">The key to retrieve the value for.</param>
		/// <param name="value">
		///   An out parameter that will contain the requested value if the key
		///   is present in the dictionary, or default(TValue) if the key is not present.
		/// </param>
		/// <returns>
		///   True if the dictionary contains the given key, false if it does not.
		/// </returns>
		public bool TryGetValue(TKey key, out TValue value)
		{
			return _dictionary.TryGetValue(key, out value);
		}

		/// <summary>
		///   Creates a NotSupportedException to throw.
		/// </summary>
		/// <returns>A NotSupportedException.</returns>
		private static Exception ReadOnlyException()
		{
			return new NotSupportedException("This dictionary is read-only.");
		}
	}
}
