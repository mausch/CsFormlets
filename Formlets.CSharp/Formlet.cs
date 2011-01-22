using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using System.Xml.Linq;

namespace Formlets.CSharp {
    public static class Formlet {
        /// <summary>
        /// A helper conversion function
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static FSharpList<Tuple<K, V>> DictToTupleList<K, V>(IEnumerable<KeyValuePair<K, V>> dict) {
            var tuples = dict.Select(kv => Tuple.Create(kv.Key, kv.Value));
            return SeqModule.ToList(tuples);
        }

        /// <summary>
        /// A text input formlet
        /// </summary>
        /// <returns></returns>
        public static Formlet<string> Input() {
            return Input("", new Dictionary<string, string>());
        }

        /// <summary>
        /// A text input formlet with a default value
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static Formlet<string> Input(string defaultValue) {
            return Input(defaultValue, new Dictionary<string, string>());
        }

        /// <summary>
        /// A text input formlet with attributes
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public static Formlet<string> Input(IEnumerable<KeyValuePair<string, string>> attributes) {
            return Input("", attributes);
        }

        /// <summary>
        /// A text input formlet with default value and attributes
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public static Formlet<string> Input(string defaultValue, IEnumerable<KeyValuePair<string, string>> attributes) {
            return new Formlet<string>(FormletModule.input(defaultValue, DictToTupleList(attributes)));
        }

        /// <summary>
        /// Lifts text into formlet
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Formlet<Unit> Text(string text) {
            return new Formlet<Unit>(FormletModule.text(text));
        }

        public static Formlet<Unit> Xml(XNode xml) {
            return new Formlet<Unit>(FormletModule.xnode(xml));
        }

        public static Formlet<FSharpFunc<A, B>> FormletFSharpFunc<A, B>(Formlet<Func<A, B>> f) {
            FSharpFunc<Func<A, B>, FSharpFunc<A, B>> toff = new FFunc<Func<A, B>, FSharpFunc<A, B>>(a => new FFunc<A, B>(a));
            var ff = FormletModule.lift(toff, f);
            return new Formlet<FSharpFunc<A, B>>(ff);
        }

        /// <summary>
        /// Applicative application (i.e. &lt;*&gt;)
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Formlet<B> Ap<A, B>(this Formlet<Func<A, B>> a, Formlet<A> b) {
            return b.Ap(a);
        }

        public static Formlet<T> Ap<T>(this Formlet<T> a, params XElement[] elem) {
            var b = a;
            foreach (var i in elem) {
                b = b.ApIgnore(Formlet.Xml(i));
            }
            return b;
        }

        public static Formlet<Func<A, B>> Ap<A, B>(this Formlet<Func<A, B>> a, string text) {
            return a.ApIgnore(Formlet.Text(text));
        }

        /// <summary>
        /// Lifts and applies a unary function
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <param name="f"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Formlet<B> Lift<A,B>(Func<A,B> f, Formlet<A> a) {
            return new Formlet<B>(FormletModule.lift(FFunc.FromFunc(f), a));
        }

        /// <summary>
        /// Lifts and applies a binary function
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <param name="f"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Formlet<C> Lift2<A,B,C>(Func<A,B,C> f, Formlet<A> a, Formlet<B> b) {
            var ff = FFunc.FromFunc(f);
            var r = FormletModule.lift2(ff, a, b);
            return new Formlet<C>(r);
        }

        /// <summary>
        /// Lifts a pure value as a formlet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Formlet<T> Yield<T>(T value) {
            var r =  FormletModule.puree(value);
            return new Formlet<T>(r);
        }
    }
}