using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Formlets.CSharp {
    public class Validator<T> : IValidator<T> {
        private readonly Func<T, bool> isValid;
        private readonly Func<T, List<XNode>, IEnumerable<XNode>> errorXml;
        private readonly Func<T, IEnumerable<string>> errorMsg;

        public Validator(Func<T, bool> isValid, Func<T, List<XNode>, IEnumerable<XNode>> errorXml, Func<T, IEnumerable<string>> errorMsg) {
            this.isValid = isValid;
            this.errorXml = errorXml;
            this.errorMsg = errorMsg;
        }

        public bool IsValid(T value) {
            return isValid(value);
        }

        public IEnumerable<XNode> BuildErrorForm(T value, List<XNode> form) {
            return errorXml(value, form);
        }

        public IEnumerable<string> ErrorMessages(T value) {
            return errorMsg(value);
        }
    }
}