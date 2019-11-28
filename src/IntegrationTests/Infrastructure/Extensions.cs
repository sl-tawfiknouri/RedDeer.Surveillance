using System;
using System.Collections.Generic;
using System.Text;

namespace RedDeer.Surveillance.IntegrationTests.Infrastructure
{
    public static class Extensions
    {
        public static bool AddIfNotExists<K, V>(this IDictionary<K, V>  dictionary, K key, V value)
        {
            if (dictionary.ContainsKey(key))
            {
                return false;
            }

            return dictionary.TryAdd(key, value);
        }

        public static V ValueOrNull<K, V>(this IDictionary<K, V> dictionary, K key) where V : class
        {
            if (dictionary.ContainsKey(key))
            {
                return dictionary[key];
            }

            return null;
        }

        public static T[] CreateArray<T>(this T item)
        {
            if (item == null)
            {
                return new T[0];
            }
            return new[] { item };
        }
    }
}
