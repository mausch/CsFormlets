using System.Linq;
using System.Xml.Linq;

namespace Formlets.CSharp {
    public static class X {
        /// <summary>
        /// Shortcut to build a <see cref="XAttribute"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static XAttribute A(string name, string value) {
            return new XAttribute(XName.Get(name), value);
        }

        /// <summary>
        /// Shortcut to build a <see cref="XElement"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static XElement E(string name, params object[] content) {
            return new XElement(XName.Get(name), content);
        }

        /// <summary>
        /// Parses raw xml
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static XNode[] Raw(string xml) {
            return XmlWriter.parseRawXml(xml).ToArray();
        }
    }
}