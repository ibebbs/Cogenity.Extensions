using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Cogenity.Extensions.Logging.EventSource
{
    public class EventLoggerFilterOptions
    {
        public IList<LoggerFilterRule> Rules { get; } = new List<LoggerFilterRule>();
    }
}
