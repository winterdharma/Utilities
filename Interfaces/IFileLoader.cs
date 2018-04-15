using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Utilities.Interfaces
{
    public interface IFileLoader
    {
        List<XmlDocument> LoadXmlFiles(string dirPath);
        List<string> LoadTextFile(string filePath, Encoding encoding);
    }
}
