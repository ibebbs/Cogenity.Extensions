using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting.Composition
{
    /// <summary>
    /// This interface is the connection between the host and the plug-in code
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// Allows the module to configure the hosting 
        /// application's <see cref="IHostBuilder"/>
        /// </summary>
        /// <param name="hostbuilder"></param>
        /// <param name="configurationSection"></param>
        void Configure(IHostBuilder hostbuilder, string configurationSection = null);
    }
}
