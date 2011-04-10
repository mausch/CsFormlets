using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Formlets.CSharp {
    public interface IFormletResult {
        IEnumerable<XNode> ErrorForm { get; }
        ICollection<string> Errors { get; }
        object Value { get; }
        Type ValueType { get; }
    }
}
