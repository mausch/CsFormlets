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
        public static Formlet<B> Ap<A,B>(this Formlet<Func<A,B>> a, Formlet<A> b) {
            return b.Ap(a);
        }

        public static Formlet<Func<A,B>> Ap<A,B>(this Formlet<Func<A,B>> a, params XElement[] elem) {
            var b = a;
            foreach (var i in elem) {
                b = b.ApIgnore(Formlet.Xml(i));
            }
            return b;
        }

        public static Formlet<Func<A,B>> Ap<A,B>(this Formlet<Func<A,B>> a, string text) {
            return a.ApIgnore(Formlet.Text(text));
        }
    }
}