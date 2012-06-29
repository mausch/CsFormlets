using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using System.Xml.Linq;
using System.Collections.Specialized;
using System.Web;
using FSharpx;

namespace Formlets.CSharp {
    public class Formlet<T>: IFormlet {
        // real formlet, wrapped
        private readonly FSharpFunc<int, Tuple<Tuple<FSharpList<XNode>, FSharpFunc<FSharpList<Tuple<string, InputValue>>, Tuple<FSharpList<XNode>, Tuple<FSharpList<string>, FSharpOption<T>>>>>, int>> f;

        /// <summary>
        /// Creates a <see cref="Formlet{T}"/> by wrapping an F# formlet
        /// </summary>
        /// <param name="f">F# formlet</param>
        public Formlet(FSharpFunc<int, Tuple<Tuple<FSharpList<XNode>, FSharpFunc<FSharpList<Tuple<string, InputValue>>, Tuple<FSharpList<XNode>, Tuple<FSharpList<string>, FSharpOption<T>>>>>, int>> f) {
            this.f = f;
        }

        public static Formlet<T> FromFsFormlet(FSharpFunc<int, Tuple<Tuple<FSharpList<XNode>, FSharpFunc<FSharpList<Tuple<string, InputValue>>, Tuple<FSharpList<XNode>, Tuple<FSharpList<string>, FSharpOption<T>>>>>, int>> f) {
            return new Formlet<T>(f);
        }

        public static implicit operator FSharpFunc<int, Tuple<Tuple<FSharpList<XNode>, FSharpFunc<FSharpList<Tuple<string, InputValue>>, Tuple<FSharpList<XNode>, Tuple<FSharpList<string>, FSharpOption<T>>>>>, int>>(Formlet<T> f) {
            return f.f;
        }

        /// <summary>
        /// Applicative application (i.e. &lt;*&gt;)
        /// </summary>
        /// <typeparam name="B"></typeparam>
        /// <param name="a"></param>
        /// <returns></returns>
        public Formlet<B> Ap<B>(Formlet<Func<T, B>> a) {
            var ff = a.Select(FSharpFunc.FromFunc);
            var r = FormletModule.ap(f, ff.f);
            return new Formlet<B>(r);
        }

        public Formlet<T> Ap(string text) {
            return ApIgnore(Formlet.Text(text));
        }

        /// <summary>
        /// Applicative left application (i.e. &lt;*)
        /// </summary>
        /// <typeparam name="C"></typeparam>
        /// <param name="a"></param>
        /// <returns></returns>
        public Formlet<T> ApIgnore<C>(Formlet<C> a) {
            var r = FormletModule.apl(f, a.f);
            return new Formlet<T>(r);
        }

        public Formlet<T> WrapWith(XElement xml) {
            var r = FormletModule.tag(xml.Name.LocalName, XmlHelpers.getAttr(xml), f);
            return new Formlet<T>(r);
        }

        /// <summary>
        /// Lifts and applies a function
        /// </summary>
        /// <typeparam name="B"></typeparam>
        /// <param name="a"></param>
        /// <returns></returns>
        public Formlet<B> Select<B>(Func<T, B> a) {
            var ff = FSharpFunc.FromFunc(a);
            var r = FormletModule.map(ff, f);
            return new Formlet<B>(r);
        }

        /// <summary>
        /// Runs a formlet against an environment
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        public FormletResult<T> Run(IEnumerable<KeyValuePair<string, string>> env) {
            var tuples = env.Select(kv => Tuple.Create(kv.Key, InputValue.NewValue(kv.Value)));
            return Run(tuples.ToFSharpList());
        }

        IFormletResult IFormlet.Run(IEnumerable<KeyValuePair<string, string>> env) {
            return Run(env);
        }

        public FormletResult<T> Run(IEnumerable<KeyValuePair<string, InputValue>> env) {
            return Run(env.ToTuples().ToFSharpList());
        }

        public FormletResult<T> Run(IEnumerable<string> values) {
            return Run(EnvDictModule.fromStrings(values));
        }

        /// <summary>
        /// Runs a formlet against an environment
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private FormletResult<T> Run(FSharpList<Tuple<string,InputValue>> list) {
            var ff = FormletModule.run(f);
            var r = ff.Invoke(list);
            return new FormletResult<T>(r.Item1, r.Item2.ToList(), r.Item3);
        }

        /// <summary>
        /// Runs a formlet against an environment
        /// </summary>
        /// <param name="nv"></param>
        /// <returns></returns>
        public FormletResult<T> Run(NameValueCollection nv) {
            var list = EnvDictModule.fromNV(nv);
            return Run(list);
        }

        IFormletResult IFormlet.Run(NameValueCollection nv) {
            return Run(nv);
        }

        public FormletResult<T> RunPost(HttpRequestBase request) {
            var list = EnvDictModule.fromNV(request.Form);
            var files = request.Files.AllKeys.Select(k => Tuple.Create(k, request.Files[k]));
            list = EnvDictModule.addFromFileSeq(files).Invoke(list);
            return Run(list);
        }

        /// <summary>
        /// Renders a formlet
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return FormletModule.render(f);
        }

        /// <summary>
        /// Renders a formlet to <see cref="XNode"/>
        /// </summary>
        /// <returns></returns>
        public XNode[] RenderToXml() {
            return FormletModule.renderToXml(f).ToArray();
        }

        public Formlet<T> Where(Func<T,bool> pred) {
            return Satisfies(pred, (v, n) => n.Append("Invalid value"), v => new[] {"Invalid value"});
        }

        public Formlet<R> Join<I, K, R>(Formlet<I> inner, Func<T, K> outerKeySelector, Func<I, K> innerKeySelector, Func<T, I, R> resultSelector) {
            return Formlet.Tuple2<T, I>()
                .Ap(this)
                .Ap(inner)
                .Select(t => resultSelector(t.Item1, t.Item2));
        }

        public Formlet<R> SelectMany<R>(Func<Formlet<T>, Formlet<R>> collector) {
            return collector(this);
        }

        public Formlet<R> SelectMany<U, R>(Func<Formlet<T>, Formlet<U>> collector, Func<T, U, R> selector) {
            return this.Join<U,object,R>(collector(this), x => x, x => x, selector);
        }

        public Formlet<T> Satisfies(Func<T,bool> pred, Func<T, List<XNode>, IEnumerable<XNode>> error, Func<T, IEnumerable<string>> errorMsg) {
            var validator = new Validator<T>(pred, error, errorMsg);
            var fsValidator = validator.ToFsValidator();
            var r = FormletModule.satisfies(fsValidator, this.f);
            return new Formlet<T>(r);
        }

        public Formlet<T> Satisfies(IValidator<T> v) {
            return Satisfies(v.IsValid, v.BuildErrorForm, v.ErrorMessages);
        }

        public Formlet<T> WithLabel(string text) {
            var e = new Formlets.FormElements(Validate.Default);
            var r = e.WithLabel(text, this.f);
            return new Formlet<T>(r);
        }

        public Formlet<T> WrapWithLabel(string xml) {
            var f = Formlet.Yield(L.F((T x) => x))
                        .Ap(XmlWriter.parseRawXml(xml).ToArray())
                        .Ap(this);
            return Formlet.Tag(X.E("label"), f);
        }

        public Formlet<T> WithLabelRaw(string xml) {
            var e = new Formlets.FormElements(Validate.Default);
            var r = e.WithLabelRaw(xml, this.f);
            return new Formlet<T>(r);
        }

        public Formlet<U> Transform<U>(Func<Formlet<T>,Formlet<U>> func) {
            return func(this);
        }

        public Formlet<U> Transform<U>(Func<FSharpFunc<int, Tuple<Tuple<FSharpList<XNode>, FSharpFunc<FSharpList<Tuple<string, InputValue>>, Tuple<FSharpList<XNode>, Tuple<FSharpList<string>, FSharpOption<T>>>>>, int>>, FSharpFunc<int, Tuple<Tuple<FSharpList<XNode>, FSharpFunc<FSharpList<Tuple<string, InputValue>>, Tuple<FSharpList<XNode>, Tuple<FSharpList<string>, FSharpOption<U>>>>>, int>>> func) {
            var ff = func(this.f);
            return new Formlet<U>(ff);
        }
    }
}