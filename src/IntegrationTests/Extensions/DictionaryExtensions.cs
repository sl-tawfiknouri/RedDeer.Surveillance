using System;
using System.Collections.Generic;
using System.Text;

namespace RedDeer.Surveillance.IntegrationTests.Extensions
{
    public static class DictionaryExtensions
    {
        public static bool AddIfNotExists<K, V>(this IDictionary<K, V>  dictionary, K key, V value)
        {
            if (dictionary.ContainsKey(key))
            {
                return false;
            }

            return dictionary.TryAdd(key, value);
        }
    }
}
