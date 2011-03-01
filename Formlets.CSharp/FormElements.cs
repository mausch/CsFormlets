using System.Collections.Generic;

namespace Formlets.CSharp {
    public class FormElements {
        private readonly Formlets.FormElements e;

        public FormElements() {
            e = new Formlets.FormElements(Validate.Default);
        }

        public FormElements(IValidate v) {
            e = new Formlets.FormElements(v);
        }

        public Formlet<bool> Checkbox(bool value, bool required = false, IEnumerable<KeyValuePair<string, string>> attributes = null) {
            attributes = attributes ?? new Dictionary<string, string>();
            var f = e.Checkbox(value, required.ToOption(), attributes.ToTuples().ToFsList().ToOption());
            return new Formlet<bool>(f);
        }

        public Formlet<string> Textarea(string value = "", IEnumerable<KeyValuePair<string,string>> attributes = null) {
            attributes = attributes ?? new Dictionary<string, string>();
            var f = e.Textarea(value.ToOption(), attributes.ToTuples().ToFsList().ToOption());
            return new Formlet<string>(f);
        }

        public Formlet<string> Text(string value = "", bool required = false, int? maxlength = null, string pattern = null, IEnumerable<KeyValuePair<string,string>> attributes = null) {
            attributes = attributes ?? new Dictionary<string, string>();
            var f = e.Text(value.ToOption(), attributes.ToTuples().ToFsList().ToOption(), required.ToOption(), maxlength.ToOption(), pattern.ToOption());
            return new Formlet<string>(f);
        }

        public Formlet<double> Double(double? value = null, bool required = false, int? maxlength = null, double? min = null, double? max = null, IEnumerable<KeyValuePair<string,string>> attributes = null) {
            attributes = attributes ?? new Dictionary<string, string>();
            var f = e.Float(value.ToOption(), attributes.ToTuples().ToFsList().ToOption(), required.ToOption(), maxlength.ToOption(), min.ToOption(), max.ToOption());
            return new Formlet<double>(f);
        }

        public Formlet<int> Int(int? value = null, bool required = false, int? maxlength = null, int? min = null, int? max = null, IEnumerable<KeyValuePair<string,string>> attributes = null) {
            attributes = attributes ?? new Dictionary<string, string>();
            var f = e.Int(value.ToOption(), attributes.ToTuples().ToFsList().ToOption(), required.ToOption(), maxlength.ToOption(), min.ToOption(), max.ToOption());
            return new Formlet<int>(f);
        }


    }
}