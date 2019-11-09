using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GenericHostConsole.Ping
{
    public class Service : IHostedService
    {
        private readonly Common.IEventBus _eventBus;
        private Timer _timer;

        public Service(Common.IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(
                _ => _eventBus.Ping(),
                null,
                TimeSpan.FromSeconds(3),
                TimeSpan.FromSeconds(3)
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
