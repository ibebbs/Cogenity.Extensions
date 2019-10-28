using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting.PlugIns
{
    /// <summary>
    /// This interface is the connection between the host and the plug-in code
    /// </summary>
    public interface IPlugIn
    {
        /// <summary>
        /// Implementing this method allows a plug-in to configure the host.
        /// This makes it possible to add services etc
        /// </summary>
        /// <param name="hostBuilderContext">HostBuilderContext</param>
        /// <param name="serviceCollection">IServiceCollection</param>
        /// <param name="configurationSection"></param>
        void ConfigureHost(HostBuilderContext hostBuilderContext, IServiceCollection serviceCollection, string configurationSection = null);
    }
}
