using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using System.Xml.Linq;
using System.Web;

namespace Formlets.CSharp {
    public static class Formlet {
        public static Formlet<T> Tag<T>(XElement elem, Formlet<T> formlet) {
            return formlet.WrapWith(elem);
        }

        /// <summary>
        /// A text input formlet with default value and attributes
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public static Formlet<string> Input(string defaultValue = "", IEnumerable<KeyValuePair<string, string>> attributes = null) {
            defaultValue = defaultValue ?? "";
            attributes = attributes ?? new Dictionary<string, string>();
            return new Formlet<string>(FormletModule.input(defaultValue, attributes.ToTuples().ToFsList()));
        }

        public static Formlet<string> Password() {
            return new Formlet<string>(FormletModule.password);
        }

        public static Formlet<string> Hidden(string defaultValue = "") {
            return new Formlet<string>(FormletModule.hidden(defaultValue));
        }

        public static Formlet<bool> CheckBox(bool defaultValue = false, IEnumerable<KeyValuePair<string, string>> attributes = null) {
            attributes = attributes ?? new Dictionary<string, string>();
            return new Formlet<bool>(FormletModule.checkbox(defaultValue, attributes.ToTuples().ToFsList()));
        }

        public static Formlet<string> Radio(string selected, IEnumerable<KeyValuePair<string,string>> values) {
            var r = FormletModule.radio(selected, values.ToTuples());
            return new Formlet<string>(r);
        }

        public static Formlet<T> Radio<T>(T selected, IEnumerable<KeyValuePair<T,string>> values) {
            var r = FormletModule.radioA(selected, values.ToTuples());
            return new Formlet<T>(r);
        }

        public static Formlet<string> Select(string selected, IEnumerable<KeyValuePair<string,string>> values) {
            var r = FormletModule.select(selected, values.ToTuples());
            return new Formlet<string>(r);
        }

        public static Formlet<T> Select<T>(T selected, IEnumerable<KeyValuePair<T, string>> values) {
            var r = FormletModule.selectA(selected, values.ToTuples());
            return new Formlet<T>(r);
        }

        public static Formlet<ICollection<string>> Select(IEnumerable<string> selected, IEnumerable<KeyValuePair<string,string>> values) {
            var r = FormletModule.selectMulti(selected, values.ToTuples());
            var f = new Formlet<FSharpList<string>>(r);
            return f.Select(l => (ICollection<string>)l.ToList());
        }

        public static Formlet<ICollection<T>> Select<T>(IEnumerable<T> selected, IEnumerable<KeyValuePair<T, string>> values) {
            var r = FormletModule.selectMultiA(selected, values.ToTuples());
            var f = new Formlet<FSharpList<T>>(r);
            return f.Select(l => (ICollection<T>)l.ToList());
        }

        public static Formlet<string> TextArea(string defaultValue = "", IEnumerable<KeyValuePair<string,string>> attributes = null) {
            attributes = attributes ?? new Dictionary<string, string>();
            var r = FormletModule.textarea(defaultValue).Invoke(attributes.ToTuples().ToFsList());
            return new Formlet<string>(r);
        }

        public static Formlet<FSharpOption<HttpPostedFileBase>> File(IEnumerable<KeyValuePair<string, string>> attributes = null) {
            attributes = attributes ?? new Dictionary<string, string>();
            var f = FormletModule.file(attributes.ToTuples().ToFsList());
            return new Formlet<FSharpOption<HttpPostedFileBase>>(f);
        }

        public static Func<string, Formlet<Unit>> ReCaptcha(string publicKey, string privateKey, bool? mockedValue = null) {
            var value = mockedValue == null ? FSharpOption<bool>.None : FSharpOption<bool>.Some(mockedValue.Value);
            var settings = new Formlets.FormletModule.ReCaptchaSettings(publicKey, privateKey, value);
            return ip => new Formlet<Unit>(FormletModule.reCaptcha(settings, ip));
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

        public static Formlet<Unit> RawXml(string xml) {
            return new Formlet<Unit>(FormletModule.rawXml(xml));
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

        public static Formlet<T> Satisfies<T>(this Formlet<T> f, Func<T, bool> pred, string errorMessage) {
            return f.Satisfies(pred, _ => errorMessage);
        }

        public static Formlet<T> Satisfies<T>(this Formlet<T> f, Func<T, bool> pred, Func<T, string> errorMessage) {
            return f.Satisfies(pred, (v, n) => {
                var attr = new Dictionary<string, string> { { "class", "error" } };
                var content = FsList.New(new XText(errorMessage(v)) as XNode);
                var span = XmlWriter.xelem("span", attr.ToTuples().ToFsList(), content);
                n.Add(span);
                return n;
            }, v => new[] {errorMessage(v)});
        }

        public static Formlet<Func<A,Func<B,Tuple<A,B>>>> Tuple2<A,B>() {
            return Yield(L.F((A a) => 
                L.F((B b) => 
                    Tuple.Create(a, b))));
        }

        public static Formlet<Func<A,Func<B,Func<C,Tuple<A,B,C>>>>> Tuple3<A,B,C>() {
            return Yield(L.F((A a) =>
                L.F((B b) =>
                    L.F((C c) => 
                        Tuple.Create(a, b, c)))));
        }

        public static Formlet<Func<A,Func<B,Func<C,Func<D,Tuple<A,B,C,D>>>>>> Tuple4<A,B,C,D>() {
            return Yield(L.F((A a) =>
                L.F((B b) =>
                    L.F((C c) => 
                        L.F((D d) =>
                            Tuple.Create(a, b, c, d))))));
        }

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