using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
using FSharpx;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;

namespace Formlets.CSharp {
    /// <summary>
    /// <see cref="IValidator{T}"/> implementation built from functions
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Validator<T> : IValidator<T> {
        private readonly Func<T, bool> isValid;
        private readonly Func<T, List<XNode>, IEnumerable<XNode>> errorXml;
        private readonly Func<T, IEnumerable<string>> errorMsg;

        /// <summary>
        /// Builds a validator from primitive functions
        /// </summary>
        /// <param name="isValid">Validates value</param>
        /// <param name="errorXml">Alters HTML adding error message</param>
        /// <param name="errorMsg">Builds error list</param>
        public Validator(Func<T, bool> isValid, Func<T, List<XNode>, IEnumerable<XNode>> errorXml, Func<T, IEnumerable<string>> errorMsg) {
            this.isValid = isValid;
            this.errorXml = errorXml;
            this.errorMsg = errorMsg;
        }

        /// <summary>
        /// Builds a validator from a FsFormlets validator
        /// </summary>
        /// <param name="validator"></param>
        public Validator(Formlets.Validator<T> validator) {
            this.isValid = validator.IsValid.Invoke;
            this.errorXml = (a, b) => validator.ErrorForm.Invoke(a).Invoke(b.ToFSharpList());
            this.errorMsg = validator.ErrorList.Invoke;
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

        /// <summary>
        /// Converts to a FsFormlets validator
        /// </summary>
        /// <returns></returns>
        public Formlets.Validator<T> ToFsValidator() {
            var f1 = FSharpFunc.FromFunc((T x, FSharpList<XNode> y) => errorXml(x, y.ToList()).ToFSharpList());
            var fpred = FSharpFunc.FromFunc(isValid);
            var fmsg = FSharpFunc.FromFunc((T x) => errorMsg(x).ToFSharpList());
            return new Formlets.Validator<T>(fpred,f1,fmsg);
        }
    }
}