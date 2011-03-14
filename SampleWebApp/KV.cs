using System.Collections.Generic;

namespace SampleWebApp {
    public static class KV {
        public static KeyValuePair<K, V> Create<K, V>(K key, V value) {
            return new KeyValuePair<K, V>(key, value);
        }
    }
}