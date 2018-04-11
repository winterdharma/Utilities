using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Utilities.IOFunctions
{
    public static class LoadFiles
    {
        /// <summary>
        /// Searches supplied directory for files with "*.xml" extension and returns
        /// a collection of XmlDocument objects. Invalid xml characters are escaped
        /// before XmlDocument objects are created.
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public static List<XmlDocument> XmlFiles(string dirPath)
        {
            if(Directory.Exists(dirPath))
            {
                var xmlDocs = new List<XmlDocument>();

                var xmlFiles = Directory.GetFiles(dirPath + "\\", "*.xml").ToList();

                ValidateXmlFiles(xmlFiles);

                foreach (string file in xmlFiles)
                {
                    XmlDocument xml = new XmlDocument();
                    xml.Load(file);
                    xmlDocs.Add(xml);
                }

                return xmlDocs;
            }
            else
            {
                throw new ArgumentException("Directory path is not valid.");
            }
        }

        private static void ValidateXmlFiles(List<string> xmlFilePaths)
        {
            foreach (string xmlFile in xmlFilePaths)
            {
                string str = File.ReadAllText(xmlFile);
                str = str.Replace("&", "&amp;");
                str = str.Replace("&amp;amp;", "&amp;");
                str = str.Replace("'", "&apos;");
                str = str.Replace("\"", "&quot;");
                //str = str.Replace(">", "&gt;");
                //str = str.Replace("<", "&lt;");
                File.WriteAllText(xmlFile, str);
            }
        }

        /// <summary>
        /// Loads each line of a file into a collection and returns a List<string> object.
        /// Throws an ArgumentException if the path supplied is not valid.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static List<string> TextFile(string filePath, Encoding encoding)
        {
            
            if (Directory.Exists(filePath))
            {
                var textFile = new List<string>();

                StreamReader input = new StreamReader(filePath, encoding);
                string term = "";
                while ((term = input.ReadLine()) != null)
                {
                    if (!term.Equals(""))
                        textFile.Add(term);
                }
                input.Close();

                return textFile;
            }
            else
            {
                throw new ArgumentException("File path is not valid.");
            }
        }
    }
}
