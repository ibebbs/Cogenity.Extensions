using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Linq;

namespace Microsoft.Extensions.Hosting.PlugIns
{
    public static class HostBuilderExtensions
    {
        private static bool Validation(Configuration.Instance options)
        {
            return true;
        }

        private static void LoadModules(HostBuilderContext hostContext, IServiceCollection serviceCollection, string configurationSection)
        {
            var optionsFactory = new OptionsFactory<Configuration.Instance>(
                new[] { new NamedConfigureFromConfigurationOptions<Configuration.Instance>(Options.Options.DefaultName, hostContext.Configuration.GetSection(configurationSection)) },
                Enumerable.Empty<IPostConfigureOptions<Configuration.Instance>>()
            );

            var configuration = optionsFactory.Create(Options.Options.DefaultName);

            new PlugIn.Loader(configuration).Load(hostContext, serviceCollection);
        }

        public static IHostBuilder ConfigurePlugIns(this IHostBuilder hostBuilder, string configurationSection)
        {
            return hostBuilder.ConfigureServices((hostingContext, serviceCollection) => LoadModules(hostingContext, serviceCollection, configurationSection));
        }
    }
}
