using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Composition;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GenericHostConsole
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            var builder = ComposableHost.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(config => config.AddCommandLine(args).AddYamlFile(args[0]))
                .ConfigureLogging((hostContext, logging) => logging.AddConsole())
                .ConfigureServices(
                    services =>
                    {
                        services.AddSingleton<Common.IEventBus, Common.EventBus>();
                        services.AddSingleton<IHostedService, Ping.Service>();
                    });

            await builder
                .Build()
                .RunAsync();
        }
    }
}
