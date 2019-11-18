 using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading;

namespace Cogenity.Extensions.Logging.EventSource
{
    public class EventLogger : EventListener
    {
        private const int Disabled = 0;
        private const int Enabled = 1;

        private readonly ConcurrentDictionary<string, ILogger> _loggers = new ConcurrentDictionary<string, ILogger>();
        private readonly ConcurrentDictionary<string, System.Diagnostics.Tracing.EventSource> _eventSources = new ConcurrentDictionary<string, System.Diagnostics.Tracing.EventSource>();

        private readonly EventLoggerFilterOptions _options;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<EventLogger> _logger;

        private int _enabled = Disabled;

        public EventLogger(IOptions<EventLoggerFilterOptions> options, ILoggerFactory loggerFactory)
        {
            _options = options.Value;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<EventLogger>();
        }

        private void EnableEventSource(System.Diagnostics.Tracing.EventSource eventSource)
        {
            if (_enabled == Enabled)
            {
                _logger.LogDebug($"Enabling EventSource '{eventSource.Name}'");

                var option = (_options?.Rules ?? Enumerable.Empty<LoggerFilterRule>())
                    .Where(rule => !string.IsNullOrWhiteSpace(rule.CategoryName) && eventSource.Name.StartsWith(rule.CategoryName, StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(rule => rule.CategoryName.Length)
                    .FirstOrDefault();

                if (option != null && (option.LogLevel ?? LogLevel.None) != LogLevel.None)
                {
                    _logger.LogInformation($"Enabling logging for EventSource '{eventSource.Name} at level '{option.LogLevel}'");

                    var logger = _loggerFactory.CreateLogger(eventSource.Name);

                    _loggers.TryAdd(eventSource.Name, logger);

                    EnableEvents(eventSource, option.LogLevel.ToEventLevel());
                }
                else
                {
                    _logger.LogInformation($"No option specified for EventSource '{eventSource.Name}'. No logging will be performed.");
                }
            }
        }

        private void DisableEventSource(System.Diagnostics.Tracing.EventSource eventSource)
        {
            _loggers.TryRemove(eventSource.Name, out ILogger _);

            DisableEvents(eventSource);
        }

        protected override void OnEventSourceCreated(System.Diagnostics.Tracing.EventSource eventSource)
        {
            _eventSources.TryAdd(eventSource.Name, eventSource);

            EnableEventSource(eventSource);
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            if (_loggers.TryGetValue(eventData.EventSource.Name, out ILogger logger))
            {
                logger.Log(eventData.Level.ToLogLevel(), eventData.Message, eventData.Payload.ToArray());                
            }
        }

        public void Enable()
        {
            if (Interlocked.CompareExchange(ref _enabled, Enabled, Disabled) == Disabled)
            {
                foreach (System.Diagnostics.Tracing.EventSource eventSource in _eventSources.Values)
                {
                    EnableEventSource(eventSource);
                }
            }
        }

        public void Disable()
        {
            if (Interlocked.CompareExchange(ref _enabled, Disabled, Enabled) == Enabled)
            {
                foreach (System.Diagnostics.Tracing.EventSource eventSource in _eventSources.Values)
                {
                    DisableEventSource(eventSource);
                }
            }
        }
    }
}
