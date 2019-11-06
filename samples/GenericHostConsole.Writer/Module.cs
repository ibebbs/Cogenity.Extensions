using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Composition;

namespace GenericHostConsole.Writer
{
    public class Module : IModule
    {
        public void Configure(IHostBuilder hostbuilder, string configurationSection = null)
        {
            hostbuilder.ConfigureServices(
                (hostBuilderContext, serviceCollection) =>
                {
                    serviceCollection.AddOptions<Configuration>().Bind(hostBuilderContext.Configuration.GetSection(configurationSection));
                    serviceCollection.AddSingleton<IHostedService, Service>();
                }
            );
        }
    }
}
