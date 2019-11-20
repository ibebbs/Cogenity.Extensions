using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Cogenity.Extensions.Logging.EventSource
{
    public class EventLoggerFilterOptions
    {
        public bool CaptureScopes { get; set; } = true;

        public IList<LoggerFilterRule> Rules { get; } = new List<LoggerFilterRule>();
    }
}
