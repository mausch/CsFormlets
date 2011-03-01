using System;
using System.Collections.Generic;
using Attributes = System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, string>>;
using Microsoft.FSharp.Core;
using Microsoft.FSharp.Collections;

namespace Formlets.CSharp {
    public class FormElements {
        private readonly Formlets.FormElements e;

        public FormElements() {
            e = new Formlets.FormElements(Validate.Default);
        }

        public FormElements(IValidate v) {
            e = new Formlets.FormElements(v);
        }

        private FSharpOption<FSharpList<Tuple<string,string>>> ToAttr(Attributes attr) {
            attr = attr ?? new Dictionary<string, string>();
            return attr.ToTuples().ToFsList().ToOption();
        }

        public Formlet<bool> Checkbox(bool value, bool required = false, Attributes attributes = null) {
            var f = e.Checkbox(value, required.ToOption(), ToAttr(attributes));
            return new Formlet<bool>(f);
        }

        public Formlet<string> Textarea(string value = "", Attributes attributes = null) {
            var f = e.Textarea(value.ToOption(), ToAttr(attributes));
            return new Formlet<string>(f);
        }

        public Formlet<string> Text(string value = "", bool required = false, int? maxlength = null, string pattern = null, Attributes attributes = null) {
            var f = e.Text(value.ToOption(), ToAttr(attributes), required.ToOption(), maxlength.ToOption(), pattern.ToOption());
            return new Formlet<string>(f);
        }

        public Formlet<double> Double(double? value = null, bool required = false, int? maxlength = null, double? min = null, double? max = null, Attributes attributes = null) {
            var f = e.Float(value.ToOption(), ToAttr(attributes), required.ToOption(), maxlength.ToOption(), min.ToOption(), max.ToOption());
            return new Formlet<double>(f);
        }

        public Formlet<int> Int(int? value = null, bool required = false, int? maxlength = null, int? min = null, int? max = null, Attributes attributes = null) {
            var f = e.Int(value.ToOption(), ToAttr(attributes), required.ToOption(), maxlength.ToOption(), min.ToOption(), max.ToOption());
            return new Formlet<int>(f);
        }
    }
}