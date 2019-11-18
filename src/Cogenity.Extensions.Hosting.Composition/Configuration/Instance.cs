using System.Collections.Generic;

namespace Microsoft.Extensions.Hosting.Composition.Configuration
{
    public class Instance
    {
        public string WorkingDirectory { get; set; }
        public IEnumerable<Module> Modules { get; set; }
    }
}
