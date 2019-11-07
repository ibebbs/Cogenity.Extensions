using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Composition;
using System.Threading.Tasks;

namespace GenericHostConsole
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(configurationBuilder => configurationBuilder.AddCommandLine(args))
                .UseComposition(config => config.AddYamlFile(args[0]));

            await builder
                .UseConsoleLifetime()
                .Build()
                .RunAsync();
        }
    }
}
