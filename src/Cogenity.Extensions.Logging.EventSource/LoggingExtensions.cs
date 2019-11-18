using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.Tracing;

namespace Cogenity.Extensions.Logging.EventSource
{
    public static class LoggingExtensions
    {
        private const string DefaultConfigurationSection = "EventLogging";

        public static IHostBuilder UseEventSourceLogging(this IHostBuilder hostBuilder, string configurationSection = null)
        {
            return hostBuilder.ConfigureServices(
                (hostBuilderContext, serviceCollection) =>
                {
                    serviceCollection.AddSingleton<IConfigureOptions<EventLoggerFilterOptions>>(new EventLoggerFilterConfigureOptions(hostBuilderContext.Configuration.GetSection(configurationSection ?? DefaultConfigurationSection)));
                    serviceCollection.AddOptions<EventLoggerFilterOptions>();
                    serviceCollection.AddSingleton<EventLogger>();
                    serviceCollection.AddSingleton<IHostApplicationLifetime, ApplicationLifetimeEx>();
                });
        }

        public static LogLevel ToLogLevel(this EventLevel eventLevel)
        {
            return eventLevel switch
            {
                EventLevel.Critical => LogLevel.Critical,
                EventLevel.Error => LogLevel.Error,
                EventLevel.Warning => LogLevel.Warning,
                EventLevel.Verbose => LogLevel.Debug,
                _ => LogLevel.Information,
            };
        }

        public static EventLevel ToEventLevel(this LogLevel? logLevel)
        {
            return logLevel switch
            {
                LogLevel.Trace => EventLevel.Verbose,
                LogLevel.Debug => EventLevel.Verbose,
                LogLevel.Information => EventLevel.Informational,
                LogLevel.Warning => EventLevel.Warning,
                LogLevel.Error => EventLevel.Error,
                LogLevel.Critical => EventLevel.Critical,
                _ => (EventLevel)(-1)
            };
        }
    }
}
