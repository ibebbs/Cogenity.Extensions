namespace Microsoft.Extensions.Hosting.Composition
{
    public interface IModule
    {
        /// <summary>
        /// Allows the module to configure the hosting 
        /// application's <see cref="IHostBuilder"/>
        /// </summary>
        /// <param name="hostbuilder">
        /// The generic hosts <see cref="IHostBuilder"/> instance
        /// </param>
        /// <param name="configurationSection">
        /// The configuration section, if any, to use
        /// </param>
        void Configure(IHostBuilder hostbuilder, string configurationSection);
    }
}
