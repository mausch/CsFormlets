﻿using System;
using System.Collections.Generic;
using KV = System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, string>>;
using Microsoft.FSharp.Core;
using Microsoft.FSharp.Collections;
using System.Web;
using System.Linq;
using System.Drawing;

namespace Formlets.CSharp {
    /// <summary>
    /// Validated form elements.
    /// These generate the appropriate HTML5 element so you can also use HTML5 for client-side validation
    /// </summary>
    public class FormElements {
        private readonly Formlets.FormElements e;

        public FormElements() {
            e = new Formlets.FormElements(Formlets.Validate.Default);
        }

        public FormElements(IValidationFunctions v) {
            e = new Formlets.FormElements(v);
        }

        public IValidationFunctions Validate {
            get { return e.Validate; }
        }

        private FSharpOption<FSharpList<Tuple<string,string>>> ToAttr(KV attr) {
            attr = attr ?? new Dictionary<string, string>();
            return attr.ToTuples().ToFsList().ToOption();
        }

        public Formlet<bool> Checkbox(bool value, bool required = false, KV attributes = null) {
            var f = e.Checkbox(value, required.ToOption(), ToAttr(attributes));
            return new Formlet<bool>(f);
        }

        public Formlet<string> Textarea(string value = "", KV attributes = null) {
            var f = e.Textarea(value.ToOption(), ToAttr(attributes));
            return new Formlet<string>(f);
        }

        public Formlet<string> Text(string value = "", bool required = false, int? maxlength = null, string pattern = null, KV attributes = null) {
            var f = e.Text(value.ToOption(), ToAttr(attributes), required.ToOption(), maxlength.ToOption(), pattern.ToOption());
            return new Formlet<string>(f);
        }

        public Formlet<double> Double(double? value = null, bool required = false, int? maxlength = null, double? min = null, double? max = null, KV attributes = null) {
            var f = e.Float(value.ToOption(), ToAttr(attributes), required.ToOption(), maxlength.ToOption(), min.ToOption(), max.ToOption());
            return new Formlet<double>(f);
        }

        public Formlet<int> Int(int? value = null, bool required = false, int? maxlength = null, int? min = null, int? max = null, KV attributes = null) {
            var f = e.Int(value.ToOption(), ToAttr(attributes), required.ToOption(), maxlength.ToOption(), min.ToOption(), max.ToOption());
            return new Formlet<int>(f);
        }

        public Formlet<double> DoubleRange(double min, double max, double? value = null, bool required = false, KV attributes = null) {
            var f = e.FloatRange(min, max, value.ToOption(), ToAttr(attributes), required.ToOption());
            return new Formlet<double>(f);
        }

        public Formlet<int> IntRange(int min, int max, int? value = null, bool required = false, KV attributes = null) {
            var f = e.IntRange(min, max, value.ToOption(), ToAttr(attributes), required.ToOption());
            return new Formlet<int>(f);
        }

        public Formlet<string> Url(string value = null, bool required = false, KV attributes = null) {
            var f = e.Url(value.ToOption(), ToAttr(attributes), required.ToOption());
            return new Formlet<string>(f);
        }

        public Formlet<string> Email(string value = null, bool required = false, KV attributes = null) {
            var f = e.Email(value.ToOption(), ToAttr(attributes), required.ToOption());
            return new Formlet<string>(f);
        }

        public Formlet<string> Radio(string value, KV choices) {
            var f = e.Radio(value, choices.ToTuples());
            return new Formlet<string>(f);
        }

        public Formlet<T> Radio<T>(T value, IEnumerable<KeyValuePair<T, string>> choices) {
            var f = e.Radio(value, choices.ToTuples());
            return new Formlet<T>(f);
        }

        public Formlet<string> Select(string value, KV choices) {
            var f = e.Select(value, choices.ToTuples());
            return new Formlet<string>(f);
        }

        public Formlet<T> Select<T>(T value, IEnumerable<KeyValuePair<T, string>> choices) {
            var f = e.Select(value, choices.ToTuples());
            return new Formlet<T>(f);
        }

        public Formlet<T> Select<T>(T value, IEnumerable<T> choices) {
            return Select(value, choices.Select(m => new KeyValuePair<T, string>(m, m.ToString())));
        }

        public Formlet<IEnumerable<string>> SelectMulti(IEnumerable<string> value, KV choices) {
            var f = e.SelectMulti(value, choices.ToTuples());
            return new Formlet<FSharpList<string>>(f)
                .Select(v => (IEnumerable<string>)v);
        }

        public Formlet<IEnumerable<T>> SelectMulti<T>(IEnumerable<T> value, IEnumerable<KeyValuePair<T, string>> choices) {
            var f = e.SelectMulti(value, choices.ToTuples());
            return new Formlet<FSharpList<T>>(f)
                .Select(v => (IEnumerable<T>)v);
        }

        public Formlet<HttpPostedFileBase> File(KV attributes = null) {
            var f = e.File(ToAttr(attributes));
            return new Formlet<FSharpOption<HttpPostedFileBase>>(f)
                .Select(v => v.HasValue() ? v.Value : null);
        }

        public Formlet<string> Hidden(string value = null) {
            var f = e.Hidden(value.ToOption());
            return new Formlet<string>(f);
        }

        public Formlet<string> Password(bool required = false) {
            var f = e.Password(required.ToOption());
            return new Formlet<string>(f);
        }

        public Formlet<string> Search(string value = null, bool required = false, int? maxlength = null, string pattern = null, KV attributes = null) {
            var f = e.Search(value.ToOption(), ToAttr(attributes), required.ToOption(), maxlength.ToOption(), pattern.ToOption());
            return new Formlet<string>(f);
        }

        public Formlet<string> Tel(string value = null, bool required = false, int? maxlength = null, string pattern = null, KV attributes = null) {
            var f = e.Tel(value.ToOption(), ToAttr(attributes), required.ToOption(), maxlength.ToOption(), pattern.ToOption());
            return new Formlet<string>(f);
        }

        public Formlet<DateTimeOffset> DateTime(DateTimeOffset? value = null, bool required = false, DateTimeOffset? min = null, DateTimeOffset? max = null, KV attributes = null) {
            var f = e.DateTime(value.ToOption(), ToAttr(attributes), required.ToOption(), min.ToOption(), max.ToOption());
            return new Formlet<DateTimeOffset>(f);
        }

        public Formlet<DateTime> DateTimeLocal(DateTime? value = null, bool required = false, DateTime? min = null, DateTime? max = null, KV attributes = null) {
            var f = e.DateTimeLocal(value.ToOption(), ToAttr(attributes), required.ToOption(), min.ToOption(), max.ToOption());
            return new Formlet<DateTime>(f);
        }

        public Formlet<DateTime> Date(DateTime? value = null, bool required = false, DateTime? min = null, DateTime? max = null, KV attributes = null) {
            var f = e.Date(value.ToOption(), ToAttr(attributes), required.ToOption(), min.ToOption(), max.ToOption());
            return new Formlet<DateTime>(f);
        }

        public Formlet<DateTime> Month(DateTime? value = null, bool required = false, DateTime? min = null, DateTime? max = null, KV attributes = null) {
            var f = e.Month(value.ToOption(), ToAttr(attributes), required.ToOption(), min.ToOption(), max.ToOption());
            return new Formlet<DateTime>(f);
        }

        public Formlet<DateTime> Week(DateTime? value = null, bool required = false, DateTime? min = null, DateTime? max = null, KV attributes = null) {
            var f = e.Week(value.ToOption(), ToAttr(attributes), required.ToOption(), min.ToOption(), max.ToOption());
            return new Formlet<DateTime>(f);
        }

        public Formlet<DateTime> Time(DateTime? value = null, bool required = false, DateTime? min = null, DateTime? max = null, KV attributes = null) {
            var f = e.Time(value.ToOption(), ToAttr(attributes), required.ToOption(), min.ToOption(), max.ToOption());
            return new Formlet<DateTime>(f);
        }

        public Formlet<string> Submit(string value = null, KV attributes = null) {
            var f = e.Submit(value.ToOption(), ToAttr(attributes));
            var of = new Formlet<FSharpOption<string>>(f);
            return of.Select(s => s.HasValue() ? s.Value : null);
        }

        public Formlet<Point?> Image(string src, string alt, KV attributes = null) {
            var f = e.Image(src, alt, ToAttr(attributes));
            var of = new Formlet<FSharpOption<Point>>(f);
            return of.Select(p => p.ToNullable());
        }

        public Formlet<Color> Color(Color? value = null, KV attributes = null) {
            var f = e.Color(value.ToOption(), ToAttr(attributes));
            return new Formlet<Color>(f);
        }

    }
}