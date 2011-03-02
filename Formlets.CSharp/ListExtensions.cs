using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Formlets.CSharp {
    public static class ListExtensions {
        public static List<XNode> Append<T>(this List<XNode> l, params T[] elems) where T : XNode {
            l.AddRange(elems);
            return l;
        }

        public static List<XNode> Append(this List<XNode> l, params string[] text) {
            var elems = text.Select(t => new XText(t)).ToArray();
            return l.Append(elems);
        }

        public static List<XNode> WrapWith(this List<XNode> l, XElement elem) {
            elem.Add(l);
            return new List<XNode> { elem };
        }

        public static string Render(this IEnumerable<XNode> l) {
            return XmlWriter.render(l);
        }

        public static IEnumerable<Tuple<K, V>> ToTuples<K, V>(this IEnumerable<KeyValuePair<K, V>> list) {
            if (list == null)
                return Enumerable.Empty<Tuple<K, V>>();
            return list.Select(kv => Tuple.Create(kv.Key, kv.Value));
        }
    }
}
