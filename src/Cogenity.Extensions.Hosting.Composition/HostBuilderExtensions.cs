using Microsoft.Extensions.Configuration;
using System;

namespace Microsoft.Extensions.Hosting.Composition
{
    public static class HostBuilderExtensions
    {
        public static readonly string CompositionConfigurationSection = "Composition";

        private static IConfigurationRoot Build(Action<IConfigurationBuilder> configurator)
        {
            var configurationBuilder = new ConfigurationBuilder();

            configurator.Invoke(configurationBuilder);

            return configurationBuilder.Build();
        }

        public static T Bind<T>(this IConfigurationSection configurationSection) where T : new()
        {
            var instance = new T();

            configurationSection.Bind(instance);

            return instance;
        }

        public static IHostBuilder UseComposition(this IHostBuilder hostBuilder, Action<IConfigurationBuilder> configurator, string configurationSection = null)
        {
            // Adds the configuration specified in the configurator to the host
            // for use later while the host is being built
            hostBuilder.ConfigureHostConfiguration(configurator);

            // Create a configuration root containing configuration from the configurator
            var configurationRoot = Build(configurator);

            // Get the composition configuration...
            var configuration = configurationRoot
                .GetSection(configurationSection ?? CompositionConfigurationSection)
                .Bind<Configuration.Instance>();

            // ... and use it to load all the configured modules
            return new Module.Loader(configuration).Load(hostBuilder);
        }
    }
}
