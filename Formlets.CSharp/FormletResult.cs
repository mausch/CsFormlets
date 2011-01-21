using Microsoft.FSharp.Core;
using System.Xml.Linq;

namespace Formlets.CSharp {
    /// <summary>
    /// Formlet result
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FormletResult<T> {
        private readonly XNode errorForm;
        private readonly FSharpOption<T> value;

        public FormletResult(XNode errorForm, FSharpOption<T> value) {
            this.errorForm = errorForm;
            this.value = value;
        }

        /// <summary>
        /// Error form
        /// </summary>
        public XNode ErrorForm {
            get { return errorForm; }
        }

        /// <summary>
        /// Formlet result value. If None, there was an error.
        /// </summary>
        public FSharpOption<T> Value {
            get { return value; }
        }
    }
}