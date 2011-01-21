using System;
using System.Xml.Linq;

namespace Formlets.CSharp {
    public static class FormletExtensions {
        /// <summary>
        /// Applicative application (i.e. &lt;*&gt;)
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Formlet<B> Apply<A,B>(this Formlet<Func<A,B>> a, Formlet<A> b) {
            return b.Apply(a);
        }

        public static Formlet<Func<A,B>> Apply<A,B>(this Formlet<Func<A,B>> a, XElement elem) {
            return a.ApplyIgnore(Formlet.Xml(elem));
        }
    }
}