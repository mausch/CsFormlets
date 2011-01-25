using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using System.Xml.Linq;
using System.Web;

namespace Formlets.CSharp {
    public static class Formlet {
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
            defaultValue = defaultValue ?? "";
            attributes = attributes ?? new Dictionary<string, string>();
            return new Formlet<string>(FormletModule.input(defaultValue, attributes.ToTuples().ToFsList()));
        }

        public static Formlet<string> Password() {
            return new Formlet<string>(FormletModule.password);
        }

        public static Formlet<string> Hidden() {
            return new Formlet<string>(FormletModule.hidden(""));
        }

        public static Formlet<string> Hidden(string defaultValue) {
            return new Formlet<string>(FormletModule.hidden(defaultValue));
        }

        public static Formlet<bool> CheckBox(bool defaultValue) {
            return new Formlet<bool>(FormletModule.checkbox(defaultValue));
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
            return f.Lift(l => (ICollection<string>)l.ToList());
        }

        public static Formlet<ICollection<T>> Select<T>(IEnumerable<T> selected, IEnumerable<KeyValuePair<T, string>> values) {
            var r = FormletModule.selectMultiA(selected, values.ToTuples());
            var f = new Formlet<FSharpList<T>>(r);
            return f.Lift(l => (ICollection<T>)l.ToList());
        }

        public static Formlet<string> TextArea() {
            return TextArea("", new Dictionary<string, string>());
        }

        public static Formlet<string> TextArea(string defaultValue) {
            return TextArea(defaultValue, new Dictionary<string, string>());
        }

        public static Formlet<string> TextArea(IEnumerable<KeyValuePair<string,string>> attributes) {
            return TextArea("", attributes);
        }

        public static Formlet<string> TextArea(string defaultValue, IEnumerable<KeyValuePair<string,string>> attributes) {
            var r = FormletModule.textarea(defaultValue).Invoke(attributes.ToTuples().ToFsList());
            return new Formlet<string>(r);
        }

        public static Formlet<FSharpOption<HttpPostedFileBase>> File(IEnumerable<KeyValuePair<string, string>> attributes) {
            return new Formlet<FSharpOption<HttpPostedFileBase>>(FormletModule.file(attributes.ToTuples().ToFsList()));
        }

        public static Formlet<FSharpOption<HttpPostedFileBase>> File() {
            return File(new Dictionary<string, string>());
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
            return a.Lift(f);
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
            });
        }
    }
}