using System;
using System.Xml.Linq;

namespace MSTestAllureAdapter
{
    /// <summary>
    /// Convenience methods for XElement
    /// </summary>
    internal static class XElementExtensions
    {
        /// <summary>
        /// Gets the value of the element with the given name.
        /// </summary>
        /// <returns>The value if found, String.Empty otherwise.</returns>
        /// <param name="element">Element.</param>
        /// <param name="name">Name.</param>
        public static string GetSafeValue(this XElement element, XName name)
        {
            string result = String.Empty;

            element = element.Element(name);

            if (element != null)
            {
                result = element.Value;
            }

            return result;
        }

        /// <summary>
        /// Gets the attribute value with the given attributeName of the element with the given descendantElement name.
        /// </summary>
        /// <returns>The attribute value if found, String.Empty otherwise.</returns>
        /// <param name="element">Element.</param>
        /// <param name="descendantElement">Descendant element.</param>
        /// <param name="attributeName">Attribute name.</param>
        public static string GetSafeAttributeValue(this XElement element, XName descendantElement, XName attributeName)
        {
            string result = String.Empty;

            element = element.Element(descendantElement);

            if (element != null && element.Attribute(attributeName) != null)
            {
                result = element.Attribute(attributeName).Value;
            }

            return result;
        }

        /// <summary>
        /// Gets the attribute value with the given attributeName.
        /// </summary>
        /// <returns>The attribute value if found, String.Empty otherwise.</returns>
        /// <param name="element">Element.</param>
        /// <param name="attributeName">Attribute name.</param>
        public static string GetSafeAttributeValue(this XElement element, XName attributeName)
        {
            string result = String.Empty;

            if (element != null && element.Attribute(attributeName) != null)
            {
                result = element.Attribute(attributeName).Value;
            }

            return result;
        }
    }
}

