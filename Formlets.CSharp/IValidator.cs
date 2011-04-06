using System.Xml.Linq;
using System.Collections.Generic;

namespace Formlets.CSharp {
    /// <summary>
    /// Encapsulates validation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IValidator<T> {
        /// <summary>
        /// Returns true if <paramref name="value"/> is valid, otherwise false
        /// </summary>
        /// <param name="value">Value to validate</param>
        /// <returns>True if value is valid, otherwise false</returns>
        bool IsValid(T value);

        /// <summary>
        /// Builds error form from collected value and original HTML tree
        /// </summary>
        /// <param name="value"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        IEnumerable<XNode> BuildErrorForm(T value, List<XNode> form);

        /// <summary>
        /// Builds error list from collected value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IEnumerable<string> ErrorMessages(T value);
    }
}