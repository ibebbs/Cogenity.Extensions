using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;

namespace Cogenity.Extensions.Logging.EventSource
{
    public class ApplicationLifetimeEx : ApplicationLifetime
    {
        public ApplicationLifetimeEx(ILogger<ApplicationLifetime> logger, EventLogger eventLogger) : base(logger)
        {
            // Take a reference to event logger just to ensure it's not optimised away
            EventLogger = eventLogger;
        }

        public EventLogger EventLogger { get; }
    }
}
