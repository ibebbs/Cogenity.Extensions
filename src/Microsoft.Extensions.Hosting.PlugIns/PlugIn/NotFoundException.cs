using System;
using System.IO;

namespace Microsoft.Extensions.Hosting.PlugIns.PlugIn
{
    public class NotFoundException : FileNotFoundException
    {
        public NotFoundException(string name, string path, Exception innerException) : base($"Plug in with the name '{name}' not found in the path '{path}'", name, innerException)
        {
            Path = path;
        }

        public string Path { get; }
    }
}
