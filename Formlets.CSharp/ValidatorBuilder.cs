using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.FSharp.Core;
using System.Xml.Linq;

namespace Formlets.CSharp
{
    public class ValidatorBuilder: IValidatorBuilder
    {
        private readonly Func<string, List<XNode>, IEnumerable<XNode>> error;

        public ValidatorBuilder(Func<string, List<XNode>, IEnumerable<XNode>> error) {
            this.error = error;
        }

        public Formlets.Validator<T> Build<T>(FSharpFunc<T, bool> isValid, FSharpFunc<T, string> errorMsg) {
            Func<T, List<XNode>, IEnumerable<XNode>> err = (a, b) => error(errorMsg.Invoke(a), b);
            var v = new Validator<T>(isValid.Invoke, err, a => new[] {errorMsg.Invoke(a)});
            return v.ToFsValidator();
        }
    }
}
