using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Microsoft.Extensions.Hosting.Composition
{
    public static class ComposableHostBuilderExtensions
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

        public static IHostBuilder UseComposition(this IHostBuilder hostBuilder, string configurationSection = null)
        {
            hostBuilder.ConfigureServices(
                (hostContext, services) =>
                {
                    services.AddOptions<Configuration.Instance>().Bind(hostContext.Configuration.GetSection(configurationSection ?? CompositionConfigurationSection));
                }
            );

            return hostBuilder;
        }
    }
}
