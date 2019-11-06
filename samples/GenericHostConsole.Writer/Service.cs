using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GenericHostConsole.Writer
{
    public class Service : IHostedService
    {
        private readonly Configuration _configuration;
        private readonly ILogger<Service> _logger;

        private Timer _timer;

        public Service(IOptions<Configuration> configuration, ILogger<Service> logger)
        {
            _configuration = configuration.Value;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(
                _ => _logger.LogInformation("Here!"),
                null,
                TimeSpan.FromSeconds(_configuration.WriteIntervalInSeconds),
                TimeSpan.FromSeconds(_configuration.WriteIntervalInSeconds)
            );

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_timer != null)
            {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
                _timer.Dispose();
                _timer = null;
            }

            return Task.CompletedTask;
        }
    }
}
