using System.Collections.Generic;
using System.Text;
using System.Xml;
using Utilities.Interfaces;

namespace Utilities.IOFunctions
{
    public class FileLoader : IFileLoader
    {
        public List<string> LoadTextFile(string filePath, Encoding encoding)
        {
            return LoadFiles.TextFile(filePath, encoding);
        }

        public List<XmlDocument> LoadXmlFiles(string dirPath)
        {
            return LoadFiles.XmlFiles(dirPath);
        }
    }
}
