using Tracing = System.Diagnostics.Tracing;

namespace Cogenity.Extensions.Logging.EventSource
{
    [Tracing.EventSource(Name = "Cogenity-Extensions-Logging-EventSource")]
    public sealed class Trace : Tracing.EventSource
    {
        public static readonly Trace Event = new Trace();

        [Tracing.Event(1, Level = Tracing.EventLevel.Verbose, Message = "Updating EventSource '{0}'")]
        public void UpdatingEventSource(string eventSource)
        {
            if (IsEnabled(Tracing.EventLevel.Verbose, Tracing.EventKeywords.All))
            {
                WriteEvent(1, eventSource);
            }
        }

        [Tracing.Event(2, Level = Tracing.EventLevel.Informational, Message = "Enabling logging for EventSource '{0}' at level '{1}'")]
        public void EnablingEventSource(string eventSource, int eventLevel)
        {
            if (IsEnabled(Tracing.EventLevel.Informational, Tracing.EventKeywords.All))
            {
                WriteEvent(2, eventSource, eventLevel);
            }
        }

        [Tracing.NonEvent]
        public void EnablingEventSource(string eventSource, Tracing.EventLevel eventLevel)
        {
            EnablingEventSource(eventSource, (int)eventLevel);
        }

        [Tracing.Event(3, Level = Tracing.EventLevel.Informational, Message = "Disabling logging for EventSource '{0}'")]
        public void DisablingEventSource(string eventSource)
        {
            if (IsEnabled(Tracing.EventLevel.Informational, Tracing.EventKeywords.All))
            {
                WriteEvent(3, eventSource);
            }
        }
    }
}
