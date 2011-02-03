using System.Xml.Linq;
using System.Collections.Generic;

namespace Formlets.CSharp {
    public interface IValidator<T> {
        bool IsValid(T value);
        IEnumerable<XNode> BuildErrorForm(T value, List<XNode> form);
        IEnumerable<string> ErrorMessages(T value);
    }
}