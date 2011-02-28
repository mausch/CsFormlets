using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.FSharp.Collections;

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

        public Formlets.Validator<T> ToFsValidator() {
            var f1 = FFunc.FromFunc1((T x) =>
                        FFunc.FromFunc1((FSharpList<XNode> y) =>
                            errorXml(x, y.ToList()).ToFsList()));
            var fpred = FFunc.FromFunc(isValid);
            var fmsg = FFunc.FromFunc<T, FSharpList<string>>(x => errorMsg(x).ToFsList());
            return new Formlets.Validator<T>(fpred,f1,fmsg);
        }
    }
}