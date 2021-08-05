using System;
using System.Collections.Generic;

namespace Engine.Util
{
    /// <summary>
    /// This utility class allows for the implementation of double ke dictionaries to obtain values.
    /// </summary>
    /// <typeparam name="TKey1">The first key.</typeparam>
    /// <typeparam name="TKey2">The second key.</typeparam>
    /// <typeparam name="TValue">The value stored at the composition of the first and second key.</typeparam>
    public class DoubleKeyDictionary<TKey1, TKey2, TValue> : Dictionary<Tuple<TKey1, TKey2>, TValue>
    {
        /// <summary>
        /// Gets and sets the value at passed in keys. Implementation of a lookup but with 2 key indexes rather than one.
        /// </summary>
        /// <param name="key1">The first key.</param>
        /// <param name="key2">The second key.</param>
        public TValue this[TKey1 key1, TKey2 key2]
        {
            // Get the value at the tuple combination key
            get => base[Tuple.Create(key1, key2)];
            // Set the value at the tuple combination key
            set => base[Tuple.Create(key1, key2)] = value;
        }

        /// <summary>
        /// Inserts a value into the dictionary at passed in key indexes.
        /// </summary>
        /// <param name="key1">The first key.</param>
        /// <param name="key2">The second key.</param>
        /// <param name="value">The value to store.</param>
        public void Add(TKey1 key1, TKey2 key2, TValue value)
        {
            // Add a value at the tuple combination key
            base.Add(Tuple.Create(key1, key2), value);
        }

        /// <summary>
        /// Checks if the combination key is in the dictionary.
        /// </summary>
        /// <param name="key1">The first key.</param>
        /// <param name="key2">The second key.</param>
        /// <returns>True if the tuple combination is in the dictionary, otherwise false.</returns>
        public bool ContainsKey(TKey1 key1, TKey2 key2)
        {
            // Create a tuple of both keys to combine into one
            return base.ContainsKey(Tuple.Create(key1, key2));
        }
    }
}