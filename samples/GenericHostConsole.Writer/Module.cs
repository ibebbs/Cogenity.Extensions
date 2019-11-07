using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Composition;
using Microsoft.Extensions.Logging;

namespace GenericHostConsole.Writer
{
    public class Module : IModule
    {
        public IHostBuilder Configure(IHostBuilder hostbuilder, string configurationSection)
        {
            return hostbuilder
                .ConfigureServices(
                    (hostBuilderContext, serviceCollection) =>
                    {
                        serviceCollection.AddOptions<Configuration>().Bind(hostBuilderContext.Configuration.GetSection(configurationSection));
                        serviceCollection.AddSingleton<IHostedService, Service>();
                    })
                .ConfigureLogging((hostingContext, logging) => logging.AddConsole());
        }
    }
}
