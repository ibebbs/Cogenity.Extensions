using System;

namespace GenericHostConsole.Common
{
    public interface IEventBus
    {
        event EventHandler OnPing;

        void Ping();
    }

    public class EventBus : IEventBus
    {
        public event EventHandler OnPing;

        public void Ping()
        {
            OnPing?.Invoke(this, EventArgs.Empty);
        }
    }
}
