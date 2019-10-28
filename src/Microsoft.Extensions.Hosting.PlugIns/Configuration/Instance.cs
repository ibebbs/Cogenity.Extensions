using System.Collections.Generic;

namespace Microsoft.Extensions.Hosting.PlugIns.Configuration
{
    public class Instance
    {
        public IEnumerable<PlugIn> PlugIns { get; set; }
    }
}
