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
    }
}
