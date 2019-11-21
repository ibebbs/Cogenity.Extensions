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
        private readonly ConcurrentDictionary<string, ILogger> _loggers = new ConcurrentDictionary<string, ILogger>();
        private readonly ConcurrentDictionary<string, System.Diagnostics.Tracing.EventSource> _eventSources = new ConcurrentDictionary<string, System.Diagnostics.Tracing.EventSource>();

        private readonly ILoggerFactory _loggerFactory;

        private EventLoggerFilterOptions _options;

        public EventLogger(IOptionsMonitor<EventLoggerFilterOptions> options, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;

            options.OnChange((options, name) => OptionsChanged(options));
            OptionsChanged(options.CurrentValue);
        }

        private void OptionsChanged(EventLoggerFilterOptions options)
        {
            _options = options;

            foreach (var eventSource in _eventSources.Values)
            {
                UpdateEventSource(eventSource);
            }
        }

        private void UpdateEventSource(System.Diagnostics.Tracing.EventSource eventSource)
        {
            Trace.Event.UpdatingEventSource(eventSource.Name);

            var option = (_options?.Rules ?? Enumerable.Empty<LoggerFilterRule>())
                .Where(rule => !string.IsNullOrWhiteSpace(rule.CategoryName) && eventSource.Name.StartsWith(rule.CategoryName, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(rule => rule.CategoryName.Length)
                .FirstOrDefault();

            if (option != null && (option.LogLevel ?? LogLevel.None) != LogLevel.None)
            {
                var logger = _loggerFactory.CreateLogger(eventSource.Name);

                if (_loggers.TryAdd(eventSource.Name, logger))
                {
                    var eventLevel = option.LogLevel.ToEventLevel();

                    Trace.Event.EnablingEventSource(eventSource.Name, eventLevel);
                    EnableEvents(eventSource, eventLevel, EventKeywords.All);
                }

            }
            else
            {
                if (_loggers.TryRemove(eventSource.Name, out ILogger _))
                {
                    Trace.Event.DisablingEventSource(eventSource.Name);
                    DisableEvents(eventSource);
                }
            }
        }

        private string Message(EventWrittenEventArgs eventData)
        {
            return eventData.Message switch
            {
                var message when eventData.ActivityId != default && eventData.RelatedActivityId != default =>
                    $"[{ActivityPathDecoder.GetActivityPathString(eventData.RelatedActivityId)}]->[{ActivityPathDecoder.GetActivityPathString(eventData.ActivityId)}]->{message}",
                var message when eventData.ActivityId != default =>
                    $"[{ActivityPathDecoder.GetActivityPathString(eventData.ActivityId)}]->{message}",
                var message when eventData.RelatedActivityId != default =>
                    $"[{ActivityPathDecoder.GetActivityPathString(eventData.RelatedActivityId)}]->{message}",
                var message => message
            };
        }

        protected override void OnEventSourceCreated(System.Diagnostics.Tracing.EventSource eventSource)
        {
            _eventSources.TryAdd(eventSource.Name, eventSource);

            UpdateEventSource(eventSource);
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            if (_loggers.TryGetValue(eventData.EventSource.Name, out ILogger logger))
            {
                var message = _options.CaptureScopes ? Message(eventData) : eventData.Message;

                logger.Log(eventData.Level.ToLogLevel(), eventData.EventId, message, eventData.Payload.ToArray());                
            }
        }
    }
}
