using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;

namespace Cogenity.Extensions.Logging.EventSource
{
    public class ApplicationLifetimeEx : ApplicationLifetime
    {
        public ApplicationLifetimeEx(ILogger<ApplicationLifetime> logger, EventLogger eventLogger) : base(logger)
        {
            ApplicationStarted.Register(state => ((EventLogger)state).Enable(), eventLogger);
            ApplicationStopping.Register(state => ((EventLogger)state).Disable(), eventLogger);
        }
    }
}
