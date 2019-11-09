using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.Hosting.Composition
{
    public static class HostBuilderExtensions
    {
        public static readonly string CompositionConfigurationSection = "Composition";

        internal static IEnumerable<T> Do<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T item in source)
            {
                action(item);

                yield return item;
            }
        }

        private static IConfigurationRoot Build(Action<IConfigurationBuilder> configurator)
        {
            var configurationBuilder = new ConfigurationBuilder();

            configurator.Invoke(configurationBuilder);

            return configurationBuilder.Build();
        }

        private static ILogger<T> CreateLogger<T>(IConfigurationRoot configurationRoot)
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddConfiguration(configurationRoot.GetSection("Logging"))
                    .AddConsole(options => options.IncludeScopes = true)
                    .AddDebug();
            });

            return loggerFactory.CreateLogger<T>();
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

            // Create a logger to aid debugging of module loading
            var logger = CreateLogger<Module.Loader>(configurationRoot);

            // ... and use it to load all the configured modules
            return new Module.Loader(configuration, logger).Load(hostBuilder);
        }
    }
}
