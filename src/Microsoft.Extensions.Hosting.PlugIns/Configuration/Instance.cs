using System.Collections.Generic;

namespace Microsoft.Extensions.Hosting.Composition.Configuration
{
    public class Instance
    {
        public IEnumerable<Module> Modules { get; set; }
    }
}
