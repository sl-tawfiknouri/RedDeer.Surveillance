namespace TestHarness.Engine.Heartbeat
{
    using System.Timers;

    using TestHarness.Engine.Heartbeat.Interfaces;

    public class PulsatingHeartbeat : IPulsatingHeartbeat
    {
        private readonly object _lock = new object();

        private ElapsedEventHandler _handler;

        public void Dispose()
        {
        }

        public void OnBeat(ElapsedEventHandler handler)
        {
            lock (this._lock)
            {
                this._handler = handler;
            }
        }

        public void Pulse()
        {
            lock (this._lock)
            {
                this._handler?.Invoke(this, null);
            }
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }
    }
}