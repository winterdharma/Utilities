using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Utilities.Xml
{
    public static class XmlFunctions
    {
        /// <summary>
        /// Returns the InnerText of the specified Xml Element. If the Element is not found,
        /// an empty string will be returned.
        /// </summary>
        /// <param name="xmlDoc">The XmlDocument that contains the element.</param>
        /// <param name="elementPath">The XPath expression to the element desired.</param>
        /// <returns></returns>
        public static string GetStringFromXmlElement(XmlDocument xmlDoc, string elementPath)
        {
            return xmlDoc.SelectSingleNode(elementPath) != null
               ? xmlDoc.SelectSingleNode(elementPath).InnerText
               : "";
        }

        public static string GetStringFromXmlElement(XmlNode node, string elementPath)
        {
            return node.SelectSingleNode(elementPath) != null
               ? node.SelectSingleNode(elementPath).InnerText
               : "";
        }
    }
}
