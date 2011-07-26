using System;
using System.Collections.Generic;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using System.Xml.Linq;
using System.Web;
using FSharp.Core.CS;

namespace Formlets.CSharp {
    public static class Formlet {
        public static Formlet<T> Tag<T>(XElement elem, Formlet<T> formlet) {
            return formlet.WrapWith(elem);
        }

        private static FSharpList<Tuple<string, string>> ToAttr(this IEnumerable<KeyValuePair<string, string>> attributes) {
            attributes = attributes ?? new Dictionary<string, string>();
            return attributes.ToTuples().ToFsList();
        }

        /// <summary>
        /// A text input formlet with default value and attributes
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public static Formlet<string> Input(string defaultValue = "", IEnumerable<KeyValuePair<string, string>> attributes = null) {
            defaultValue = defaultValue ?? "";
            return new Formlet<string>(FormletModule.input(defaultValue, attributes.ToAttr()));
        }

        public static Formlet<string> Password() {
            return new Formlet<string>(FormletModule.password);
        }

        public static Formlet<string> Hidden(string defaultValue = "") {
            return new Formlet<string>(FormletModule.hidden(defaultValue));
        }

        public static Formlet<bool> CheckBox(bool defaultValue = false, IEnumerable<KeyValuePair<string, string>> attributes = null) {
            return new Formlet<bool>(FormletModule.checkbox(defaultValue, attributes.ToAttr()));
        }

        public static Formlet<string> Radio(string selected, IEnumerable<KeyValuePair<string,string>> values) {
            var r = FormletModule.radio(selected, values.ToTuples());
            return new Formlet<string>(r);
        }

        public static Formlet<T> Radio<T>(T selected, IEnumerable<KeyValuePair<T,string>> values) {
            var r = FormletModule.radioA(selected, values.ToTuples());
            return new Formlet<T>(r);
        }

        public static Formlet<string> Select(string selected, IEnumerable<KeyValuePair<string, string>> values, IEnumerable<KeyValuePair<string, string>> attributes = null) {
            var r = FormletModule.select(selected, values.ToTuples(), attributes.ToAttr());
            return new Formlet<string>(r);
        }

        public static Formlet<T> Select<T>(T selected, IEnumerable<KeyValuePair<T, string>> values, IEnumerable<KeyValuePair<string, string>> attributes = null) {
            var r = FormletModule.selectA(selected, values.ToTuples(), attributes.ToAttr());
            return new Formlet<T>(r);
        }

        public static Formlet<IEnumerable<string>> SelectMulti(IEnumerable<string> selected, IEnumerable<KeyValuePair<string,string>> values, IEnumerable<KeyValuePair<string, string>> attributes = null) {
            var r = FormletModule.selectMulti(selected, values.ToTuples(), attributes.ToAttr());
            var f = new Formlet<FSharpList<string>>(r);
            return f.Select(l => (IEnumerable<string>)l);
        }

        public static Formlet<IEnumerable<T>> SelectMulti<T>(IEnumerable<T> selected, IEnumerable<KeyValuePair<T, string>> values, IEnumerable<KeyValuePair<string, string>> attributes = null) {
            var r = FormletModule.selectMultiA(selected, values.ToTuples(), attributes.ToAttr());
            var f = new Formlet<FSharpList<T>>(r);
            return f.Select(l => (IEnumerable<T>)l);
        }

        public static Formlet<string> TextArea(string defaultValue = "", IEnumerable<KeyValuePair<string,string>> attributes = null) {
            var r = FormletModule.textarea(defaultValue).Invoke(attributes.ToAttr());
            return new Formlet<string>(r);
        }

        public static Formlet<FSharpOption<HttpPostedFileBase>> File(IEnumerable<KeyValuePair<string, string>> attributes = null) {
            var f = FormletModule.file(attributes.ToAttr());
            return new Formlet<FSharpOption<HttpPostedFileBase>>(f);
        }

        public static Func<string, Formlet<Unit>> ReCaptcha(string publicKey, string privateKey, bool? mockedValue = null) {
            var value = mockedValue == null ? FSharpOption<bool>.None : FSharpOption<bool>.Some(mockedValue.Value);
            var settings = new Formlets.FormletModule.ReCaptchaSettings(publicKey, privateKey, value);
            return ip => new Formlet<Unit>(FormletModule.reCaptcha(settings, ip));
        }

        public static Formlet<T> Pickle<T>(T value) {
            var f = FormletModule.pickler(value);
            return new Formlet<T>(f);
        }

        /// <summary>
        /// Lifts text into formlet
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Formlet<Unit> Text(string text) {
            return new Formlet<Unit>(FormletModule.text(text));
        }

        /// <summary>
        /// Lifts pure xml into formlet
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static Formlet<Unit> Xml(XNode xml) {
            return new Formlet<Unit>(FormletModule.xnode(xml));
        }

        /// <summary>
        /// Parses raw xml and lifts into formlet
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static Formlet<Unit> RawXml(string xml) {
            return new Formlet<Unit>(FormletModule.rawXml(xml));
        }

        /// <summary>
        /// Lifts text into formlet
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static Formlet<Unit> Raw(string text) {
            return new Formlet<Unit>(FormletModule.text(text));
        }

        /// <summary>
        /// Lifts pure xml into formlet
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static Formlet<Unit> Raw(XNode xml) {
            return new Formlet<Unit>(FormletModule.xnode(xml));
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

        public static Formlet<T> Ap<T>(this Formlet<T> a, params XNode[] elem) {
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
            return a.Select(f);
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
            var r = FormletModule.map2(ff, a, b);
            return new Formlet<C>(r);
        }

        /// <summary>
        /// Lifts a pure value as a formlet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Formlet<T> Yield<T>(T value) {
            var r = FormletModule.puree(value);
            return new Formlet<T>(r);
        }

        /// <summary>
        /// Applies validation to this formlet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="f"></param>
        /// <param name="pred"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static Formlet<T> Satisfies<T>(this Formlet<T> f, Func<T, bool> pred, string errorMessage) {
            return f.Satisfies(pred, _ => errorMessage);
        }

        /// <summary>
        /// Applies validation to this formlet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="f"></param>
        /// <param name="pred"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static Formlet<T> Satisfies<T>(this Formlet<T> f, Func<T, bool> pred, Func<T, string> errorMessage) {
            return f.Satisfies(pred, 
                (v, n) => n.Append(X.E("span", X.A("class", "error"), errorMessage(v))), 
                v => new[] {errorMessage(v)});
        }

        /// <summary>
        /// Creates a formlet to collect a single value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Formlet<Func<T,T>> Single<T>() {
            return Yield(L.F((T t) => t));
        }

        /// <summary>
        /// Creates a formlet to collect a pair of values
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <returns></returns>
        public static Formlet<Func<A,Func<B,Tuple<A,B>>>> Tuple2<A,B>() {
            return Yield(L.F((A a) => 
                L.F((B b) => 
                    Tuple.Create(a, b))));
        }

        /// <summary>
        /// Creates a formlet to collect three values in a tuple
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <returns></returns>
        public static Formlet<Func<A,Func<B,Func<C,Tuple<A,B,C>>>>> Tuple3<A,B,C>() {
            return Yield(L.F((A a) =>
                L.F((B b) =>
                    L.F((C c) => 
                        Tuple.Create(a, b, c)))));
        }

        /// <summary>
        /// Creates a formlet to collect four values in a tuple
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <typeparam name="D"></typeparam>
        /// <returns></returns>
        public static Formlet<Func<A,Func<B,Func<C,Func<D,Tuple<A,B,C,D>>>>>> Tuple4<A,B,C,D>() {
            return Yield(L.F((A a) =>
                L.F((B b) =>
                    L.F((C c) => 
                        L.F((D d) =>
                            Tuple.Create(a, b, c, d))))));
        }

        /// <summary>
        /// Creates a formlet to collect five values in a tuple
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <typeparam name="D"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <returns></returns>
        public static Formlet<Func<A, Func<B, Func<C, Func<D, Func<E, Tuple<A, B, C, D, E>>>>>>> Tuple5<A, B, C, D, E>() {
            return Yield(L.F((A a) =>
                L.F((B b) =>
                    L.F((C c) =>
                        L.F((D d) =>
                            L.F((E e) => 
                                Tuple.Create(a, b, c, d, e)))))));
        }

     }
}