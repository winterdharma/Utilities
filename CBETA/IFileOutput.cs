using System.Collections.Generic;

namespace Utilities.CBETA
{
    public interface IFileOutput
    {
        List<string> GetFileOutput();
        string OutputDirectory { get; }
        string OutputFilename { get; }
    }
}
