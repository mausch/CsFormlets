using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace Formlets.CSharp {
    public interface IFormlet {
        IFormletResult Run(IEnumerable<KeyValuePair<string, string>> env);
        IFormletResult Run(NameValueCollection nv);
    }
}
