using Microsoft.FSharp.Core;
using System.Xml.Linq;
using System.Collections.Generic;

namespace Formlets.CSharp {
    /// <summary>
    /// Formlet result
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FormletResult<T> {
        private readonly IEnumerable<XNode> errorForm;
        private readonly FSharpOption<T> value;
        private readonly ICollection<string> errors;

        public FormletResult(IEnumerable<XNode> errorForm, ICollection<string> errors, FSharpOption<T> value) {
            this.errorForm = errorForm;
            this.errors = errors;
            this.value = value;
        }

        /// <summary>
        /// Error form
        /// </summary>
        public IEnumerable<XNode> ErrorForm {
            get { return errorForm; }
        }

        /// <summary>
        /// List of errors
        /// </summary>
        public ICollection<string> Errors {
            get { return errors; }
        }

        /// <summary>
        /// Formlet result value. If None, there was an error.
        /// </summary>
        public FSharpOption<T> Value {
            get { return value; }
        }
    }
}